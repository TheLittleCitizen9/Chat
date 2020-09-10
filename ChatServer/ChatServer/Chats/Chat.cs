using BasicChatContents;
using System;

namespace ChatServer.Chats
{
    public class Chat
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public ChatOptions ChatOption { get; set; }

        public Chat(string name, Guid id, ChatOptions option)
        {
            Name = name;
            Id = id;
            ChatOption = option;
        }
    }
}
