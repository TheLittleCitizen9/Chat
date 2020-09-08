using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ChatServer.ChatManagers
{
    public class BasicChatManager
    {
        protected List<User> _clients;
        protected ConsoleDisplayer _consoleDisplayer;
        protected Dictionary<Guid, List<User>> _usersInChats;
        public BasicChatManager(Dictionary<Guid, List<User>> usersInChats, List<User> clients)
        {
            _usersInChats = usersInChats;
            _clients = clients;
            _consoleDisplayer = new ConsoleDisplayer();
        }
        public string GetDataFromClient(User user)
        {
            byte[] buffer = new byte[1024];
            NetworkStream nwStream = user.ClientSocket.GetStream();
            int bytesRecieved = nwStream.Read(buffer);
            string stringData = Encoding.ASCII.GetString(buffer);
            return stringData;
        }

        public string DisconnectClient(User user)
        {
            string clientDisconnectedMsg = $"Client {user.ClientSocket.Client.RemoteEndPoint} disconnected";
            _consoleDisplayer.PrintValueToConsole(clientDisconnectedMsg);
            RemoveClient(user);
            return clientDisconnectedMsg;
        }

        public void RemoveClient(User user, Guid chatId = default)
        {
            if (chatId == default)
            {
                RemoveUserFromAllChats(user);
            }
            else
            {
                _usersInChats[chatId].Remove(user);
            }
        }

        private void RemoveUserFromAllChats(User user)
        {
            _clients.Remove(user);
            foreach (KeyValuePair<Guid, List<User>> pair in _usersInChats)
            {
                foreach (var usr in pair.Value)
                {
                    if (usr == user)
                    {
                        pair.Value.Remove(usr);
                        break;
                    }
                }
            }
        }
    }
}
