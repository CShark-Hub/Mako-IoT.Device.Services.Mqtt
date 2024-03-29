﻿using MakoIoT.Device.Services.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace MakoIoT.Device.Services.Mqtt.Extensions
{
    public static class DeviceBuilderExtension
    {
        public static IDeviceBuilder AddMqtt(this IDeviceBuilder builder)
        {
            builder.Services.AddSingleton(typeof(ICommunicationService), typeof(MqttCommunicationService));
            return builder;
        }
    }
}
