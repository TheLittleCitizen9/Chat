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
        
        public bool CheckIfAListContainsAnother(List<User> users, List<User> otherUsers)
        {
            if(!users.Equals(otherUsers))
            {
                var result = users.Intersect(otherUsers).ToList();
                if(result.Count != users.Count)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
