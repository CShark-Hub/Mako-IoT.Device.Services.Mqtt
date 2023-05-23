using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using MakoIoT.Device.Services.Interface;
using MakoIoT.Device.Services.Mqtt.Configuration;
using MakoIoT.Device.Utilities.String.Extensions;
using Microsoft.Extensions.Logging;
using nanoFramework.M2Mqtt;
using nanoFramework.M2Mqtt.Messages;

namespace MakoIoT.Device.Services.Mqtt
{
    public class MqttCommunicationService : ICommunicationService
    {
        private const string directTopicPrefix = "direct";
        private const string broadcastTopicPrefix = "broadcast";

        private readonly INetworkProvider _networkProvider;
        private readonly ILogger _logger;
        private MqttClient _mqttClient;
        private readonly MqttConfig _config;
        private readonly X509Certificate _caCert;

        public MqttCommunicationService(INetworkProvider networkProvider, IConfigurationService configService, ILogger logger)
        {
            _networkProvider = networkProvider;
            _logger = logger;

            _config = (MqttConfig)configService.GetConfigSection(MqttConfig.SectionName, typeof(MqttConfig));
            if (!String.IsNullOrEmpty(_config.CACert))
                _caCert = new X509Certificate(_config.CACert);
        }

        public bool CanSend => _mqttClient != null && _mqttClient.IsConnected && _networkProvider.IsConnected;
        public string ClientName => _config.ClientId;
        public string ClientAddress => _networkProvider.ClientAddress;

        public event EventHandler MessageReceived;

        public void Publish(string messageString, string messageType)
        {
            string topic = $"{_config.TopicPrefix}/{broadcastTopicPrefix}/{messageType}";
            PublishInternal(messageString, topic);
        }

        public void Send(string messageString, string recipient)
        {
            string topic = $"{_config.TopicPrefix}/{directTopicPrefix}/{recipient}";
            PublishInternal(messageString, topic);
        }

        public void Connect(string[] subscriptions)
        {
            if (!_networkProvider.IsConnected)
            {
                _networkProvider.Connect();
                if (!_networkProvider.IsConnected)
                    throw new NotConnectedException("Could not connect to network");
            }

            if (_mqttClient == null)
            {
                _mqttClient = new MqttClient(_config.BrokerAddress, _config.Port, _config.UseTLS, _caCert, null, MqttSslProtocols.TLSv1_2);
                _mqttClient.MqttMsgPublishReceived += OnMessageReceived;
            }

            if (!_mqttClient.IsConnected)
            {
                var mqttConnectResult = _mqttClient.Connect(_config.ClientId, _config.Username, _config.Password);
                if (!_mqttClient.IsConnected)
                    throw new NotConnectedException($"Could not connect to MQTT. Broker returned {mqttConnectResult}");

                var topics = GetSubscriptionTopics(subscriptions);
                var qosLevels = new MqttQoSLevel[topics.Length];
                for (int i = 0; i < qosLevels.Length; i++)
                {
                    qosLevels[i] = MqttQoSLevel.AtLeastOnce;
                    _logger.LogDebug($"Subscribing to topic: {topics[i]}");
                }

                _mqttClient.Subscribe(topics, qosLevels);
            }
        }

        public void Disconnect()
        {
            _mqttClient.Disconnect();
            _networkProvider.Disconnect();
        }


        private void OnMessageReceived(object sender, MqttMsgPublishEventArgs e)
        {
            _logger.LogDebug($"Received message from topic: {e.Topic}");
            var message = Encoding.UTF8.GetString(e.Message, 0, e.Message.Length);
            _logger.LogTrace(message.EscapeForInterpolation());
            MessageReceived?.Invoke(this, new ObjectEventArgs(message));
        }

        private string[] GetSubscriptionTopics(string[] subscriptions)
        {
            var topics = new string[subscriptions.Length + 1];
            topics[0] = $"{_config.TopicPrefix}/{directTopicPrefix}/{_config.ClientId}";
            for (int i = 1; i < subscriptions.Length + 1; i++)
            {
                topics[i] = $"{_config.TopicPrefix}/{broadcastTopicPrefix}/{subscriptions[i-1]}";
            }

            return topics;
        }

        private void PublishInternal(string messageString, string topic)
        {
            _logger.LogDebug($"Publishing message to topic: {topic}");
            _mqttClient.Publish(topic, Encoding.UTF8.GetBytes(messageString), null, null, 
                (MqttQoSLevel)_config.PublishQoS, _config.PublishRetain);
        }
    }

    public class NotConnectedException : Exception
    {
        public NotConnectedException(string message) : base(message)
        {
            
        }
    }
}
