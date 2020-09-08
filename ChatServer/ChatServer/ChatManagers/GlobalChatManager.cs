using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ChatServer.ChatManagers
{
    public class GlobalChatManager : BasicChatManager
    {
        private Guid _globalChatId;
        public List<User> UsersInChat;

        public GlobalChatManager(Guid id, Dictionary<Guid, List<User>> usersInChats, List<User> clients)
            :base(usersInChats, clients)
        {
            _globalChatId = id;
            UsersInChat = new List<User>();
        }
        public void ChatWithClientInGlobalChat(User user)
        {
            try
            {
                while (true)
                {
                    string dataFromClient = GetDataFromClient(user);
                    string noNullValuesData = dataFromClient.Replace("\0", string.Empty);
                    if (noNullValuesData == "return")
                    {
                        user.AddNumbChatId(_globalChatId);
                        RemoveClient(user, _globalChatId);
                        SendMessageToAllClients($"Client {user.Id} left chat");
                        break;
                    }
                    string messageToClients = $"Client {user.Id} - {dataFromClient}";
                    SendMessageToAllClients(messageToClients);
                }
            }
            catch (Exception)
            {
                DisconnectClient(user);
            }
        }
        public void SendMessageToAllClients(string dataToSend)
        {
            byte[] data = Encoding.ASCII.GetBytes(dataToSend);
            foreach (var client in UsersInChat)
            {
                try
                {
                    NetworkStream nwStream = client.ClientSocket.GetStream();
                    nwStream.Write(data);
                }
                catch (Exception e)
                {
                    _consoleDisplayer.PrintValueToConsole(e.Message);
                }
            }
        }

        public void EnterUserToGlobalChat(User user)
        {
            user.AddActiveChatId(_globalChatId);
            UsersInChat.Add(user);
            string clientConnectedMsg = $"Client {user.Id} connected";
            SendMessageToAllClients(clientConnectedMsg);
            ChatWithClientInGlobalChat(user);
        }
    }
}
