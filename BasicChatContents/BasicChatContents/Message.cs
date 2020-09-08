using System;

namespace BasicChatContents
{
    [Serializable]
    public class Message<T>
    {
        public Guid ChatId { get; set; }
        public T MessageToSend { get; set; }
    }
}
