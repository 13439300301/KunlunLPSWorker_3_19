using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RoutingKeyAttribute : Attribute
    {
        private readonly string _messageType;

        public RoutingKeyAttribute(string messageType)
        {
            _messageType = messageType;
        }

        public string MessageType
        {
            get
            {
                return _messageType;
            }
        }
    }
}
