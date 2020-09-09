using ChatServer.Handlers;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ChatServer.ChatManagers
{
    public class GeneralChatFunctions
    {
        public List<User> Clients;
        public ConsoleDisplayer ConsoleDisplayer;
        public Dictionary<Guid, List<User>> UsersInChats;
        public List<User> ActiveUsersInChat { get; set; }

        public ClientHandler _clientHandler;
        private object _lock = new object();
        public GeneralChatFunctions(Dictionary<Guid, List<User>> usersInChats, List<User> clients, ClientHandler clientHandler)
        {
            UsersInChats = usersInChats;
            Clients = clients;
            ConsoleDisplayer = new ConsoleDisplayer();
            ActiveUsersInChat = new List<User>();
            _clientHandler = clientHandler;
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
            string clientDisconnectedMsg = $"Client {user.Id} disconnected";
            ConsoleDisplayer.PrintValueToConsole(clientDisconnectedMsg);
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
                ActiveUsersInChat.Remove(user);
            }
        }

        public User FindUser(string id)
        {
            User user = null;
            int userId = int.Parse(id);
            foreach (var client in Clients)
            {
                if (client.Id == userId)
                {
                    user = client;
                }
            }
            return user;
        }

        public bool SendAllClientsConnected(User user)
        {
            string noConnectedClients = "No other users connected";
            string allConnectedClients = string.Empty;
            lock (_lock)
            {
                foreach (var client in Clients)
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

        public void RemoveClientFromSpecificChat(User user, Guid chatId)
        {
            foreach (var usr in UsersInChats[chatId])
            {
                if (usr == user)
                {
                    UsersInChats[chatId].Remove(usr);
                    break;
                }
            }
            RemoveClient(user, chatId);
        }

        private void RemoveUserFromAllChats(User user)
        {
            Clients.Remove(user);
            foreach (KeyValuePair<Guid, List<User>> pair in UsersInChats)
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
