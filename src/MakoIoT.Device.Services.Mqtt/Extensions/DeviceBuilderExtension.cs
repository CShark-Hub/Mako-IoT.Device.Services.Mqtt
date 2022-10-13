using MakoIoT.Device.Services.DependencyInjection;
using MakoIoT.Device.Services.Interface;

namespace MakoIoT.Device.Services.Mqtt.Extensions
{
    public static class DeviceBuilderExtension
    {
        public static IDeviceBuilder AddMqtt(this IDeviceBuilder builder)
        {
            DI.RegisterSingleton(typeof(ICommunicationService), typeof(MqttCommunicationService));
            return builder;
        }
    }
}
