using System;
using System.Collections.Generic;
using System.Text;

namespace ChatClient
{
    [Serializable]
    public class Message<T>
    {
        public Guid ChatId { get; set; }
        public T MessageToSend { get; set; }
    }
}
