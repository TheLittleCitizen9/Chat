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
        public GeneralChatFunctions(Dictionary<Guid, List<User>> usersInChats, List<User> clients)
        {
            UsersInChats = usersInChats;
            Clients = clients;
            ConsoleDisplayer = new ConsoleDisplayer();
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
