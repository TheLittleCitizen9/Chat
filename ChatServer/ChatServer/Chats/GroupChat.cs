using System;
using System.Collections.Generic;
using System.Text;

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
