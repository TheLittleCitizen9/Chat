using BasicChatContents;
using System;
using System.Collections.Generic;

namespace ChatServer.Chats
{
    public class GroupChat : Chat
    {
        public List<User> Admins { get; set; }
        public GroupChat(string name, Guid id, ChatOptions option):base(name, id, option)
        {
            Admins = new List<User>();
        }
    }
}
