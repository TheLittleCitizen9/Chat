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
        public bool SendAllClientsConnected(User user)
        {
            string noConnectedClients = "No other users connected";
            string allConnectedClients = string.Empty;
            lock (_lock)
            {
                foreach (var client in _clients)
                {
                    if (client != user)
                    {
                        allConnectedClients += $"Client {client.Id},";
                    }
                }
            }
            if (string.IsNullOrEmpty(allConnectedClients))
            {
                _clientHandler.SendClientMessage(noConnectedClients, user);
                return false;
            }
            else
            {
                _clientHandler.SendClientMessage(allConnectedClients, user);
                return true;
            }
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
