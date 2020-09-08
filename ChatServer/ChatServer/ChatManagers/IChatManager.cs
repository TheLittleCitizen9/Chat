using System;
using System.Collections.Generic;
using System.Text;

namespace ChatServer.ChatManagers
{
    public interface IChatManager
    {
        public User SecondUser { get; set; }
        void RemoveClientFromReceivingMessages(User user);
        void EnterUserToChat(User user);


    }
}
