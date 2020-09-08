using System.Collections.Generic;

namespace ChatServer.ChatManagers
{
    public interface IChatManager
    {
        public List<User> OtherUsersInChat { get; set; }
        void RemoveClientFromReceivingMessages(User user);
        void EnterUserToChat(User user);


    }
}
