using System.Collections.Generic;
using System.Linq;

namespace ChatServer.Handlers
{
    public class GeneralHandler
    {
        private object _lock = new object();
        private List<User> _clients;
        private ClientHandler _clientHandler;


        public GeneralHandler(List<User> clients, ClientHandler clientHandler)
        {
            _clients = clients;
            _clientHandler = clientHandler;
        }
        
        public bool CheckIfAListContainsAnother(List<User> users, List<User> otherUsers, List<User> usersInChat)
        {
            if(!users.Equals(otherUsers))
            {
                var allUsersInChat = otherUsers.Select(u => u).ToList();
                allUsersInChat.AddRange(usersInChat);
                var result = users.Intersect(allUsersInChat).ToList();
                if(result.Count != users.Count)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
