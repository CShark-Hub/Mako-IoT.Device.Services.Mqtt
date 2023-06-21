#  Mako-IoT.Device.Services.Mqtt
ICommunicationService implementation with MQTT as transport layer. Uses [M2MQTT](https://github.com/nanoframework/nanoFramework.m2mqtt) library.

## Usage
Add MQTT and configuration in _DeviceBuilder_
```c#
DeviceBuilder.Create()
    .AddWiFi()
    .AddMqtt()
    .AddConfiguration(cfg =>
    {
        cfg.WriteDefault(WiFiConfig.SectionName, new WiFiConfig
        {
            Ssid = "",
            Password = ""
        });
        cfg.WriteDefault(MqttConfig.SectionName, new MqttConfig
        {
            BrokerAddress = "test.mosquitto.org",
            Port = 8883,
            UseTLS = true,
            CACert = "...", //broker's TLS certificate
            ClientId = "device1",
            TopicPrefix = "mako-iot-test", //prefix for pub-sub topics
            PublishQoS = 1, //QoS for published messages (0-2)
            PublishRetain = false //retain flag for published messages
        });
    })
    .Build()
    .Start();
```

### _ICommunicationService_
See example in [MessageBus class](https://github.com/CShark-Hub/Mako-IoT.Device.Services.Messaging/blob/main/src/MakoIoT.Device.Services.Messaging/MessageBus.cs).
