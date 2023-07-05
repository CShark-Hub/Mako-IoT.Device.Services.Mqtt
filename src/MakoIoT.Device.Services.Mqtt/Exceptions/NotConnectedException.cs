using System;

namespace MakoIoT.Device.Services.Mqtt.Exceptions
{
    public sealed class NotConnectedException : Exception
    {
        public NotConnectedException(string message) : base(message)
        {

        }
    }
}
