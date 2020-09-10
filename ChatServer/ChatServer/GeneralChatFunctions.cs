using BasicChatContents;
using ChatServer.Chats;
using ChatServer.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace ChatServer
{
    public class GeneralChatFunctions
    {
        public List<User> Clients;
        public ConsoleDisplayer ConsoleDisplayer;
        public Dictionary<Guid, List<User>> UsersInChats;

        public ClientHandler ClientHandler;
        private object _lock = new object();
        public GeneralChatFunctions(Dictionary<Guid, List<User>> usersInChats, List<User> clients, ClientHandler clientHandler)
        {
            UsersInChats = usersInChats;
            Clients = clients;
            ConsoleDisplayer = new ConsoleDisplayer();
            ClientHandler = clientHandler;
        }
        public string GetDataFromClient(User user)
        {
            byte[] buffer = new byte[1024];
            NetworkStream nwStream = user.ClientSocket.GetStream();
            int bytesRecieved = nwStream.Read(buffer); 
            string stringData = Encoding.ASCII.GetString(buffer);
            return stringData;
        }

        public bool CheckIfAListContainsAnother(List<User> users, List<User> otherUsers, List<User> usersInChat)
        {
            if (!users.Equals(otherUsers))
            {
                var allUsersInChat = otherUsers.Select(u => u).ToList();
                allUsersInChat.AddRange(usersInChat);
                var result = users.Intersect(allUsersInChat).ToList();
                if (result.Count != users.Count)
                {
                    return false;
                }
            }
            return true;
        }

        public string DisconnectClient(User user)
        {
            string clientDisconnectedMsg = $"Client {user.Id} disconnected";
            ConsoleDisplayer.PrintValueToConsole(clientDisconnectedMsg);
            RemoveClient(user);
            Clients.Remove(user);
            return clientDisconnectedMsg;
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
                ClientHandler.SendClientMessage(noConnectedClients, user);
                return false;
            }
            else
            {
                ClientHandler.SendClientMessage(allConnectedClients, user);
                return true;
            }
        }

        public bool SendAllClientsInChat(User user, Guid chatId)
        {
            string noConnectedClients = "No other users connected";
            string allConnectedClients = string.Empty;
            lock (_lock)
            {
                foreach (var client in UsersInChats[chatId])
                {
                    if (client != user)
                    {
                        allConnectedClients += $"Client {client.Id},";
                    }
                }
            }
            if (string.IsNullOrEmpty(allConnectedClients))
            {
                ClientHandler.SendClientMessage(noConnectedClients, user);
                return false;
            }
            else
            {
                ClientHandler.SendClientMessage(allConnectedClients, user);
                return true;
            }
        }

        public void AddChatToAllUsers(List<User> users, Chat chat)
        {
            foreach (var user in users)
            {
                user.InactiveChatIds.Add(chat.Id);
                user.AllChats.Add(chat);
            }
        }
        public void RemoveClient(User user, List<User> activeUsersInChat=null, Guid chatId = default)
        {
            if (chatId == default)
            {
                RemoveUserFromAllChats(user);
            }
            else if(activeUsersInChat != null)
            {
                activeUsersInChat.Remove(user);
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
            RemoveClient(user, chatId:chatId);
        }

        private void RemoveUserFromAllChats(User user)
        {
            Clients.Remove(user);
            foreach (KeyValuePair<Guid, List<User>> pair in UsersInChats)
            {
                RemoveClientFromSpecificChat(user, pair.Key);
            }
        }
    }
}
