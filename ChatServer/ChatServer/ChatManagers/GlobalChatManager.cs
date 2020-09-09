using BasicChatContents;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ChatServer.ChatManagers
{
    public class GlobalChatManager : IChatManager
    {
        private Guid _globalChatId;
        public List<User> UsersInChat { get; set; }
        private const string GLOBAL_CHAT_NAME = "Global";
        public GeneralChatFunctions ChatFunctions;
        public List<User> OtherUsersInChat { get; set; }

        public GlobalChatManager(Guid id, GeneralChatFunctions chatFunctions)
        {
            _globalChatId = id;
            UsersInChat = new List<User>();
            ChatFunctions = chatFunctions;
            OtherUsersInChat = new List<User>();
        }
        public void ChatWithClient(User user)
        {
            try
            {
                while (true)
                {
                    string dataFromClient = ChatFunctions.GetDataFromClient(user);
                    string noNullValuesData = dataFromClient.Replace("\0", string.Empty);
                    if (noNullValuesData == "return")
                    {
                        RemoveClientFromReceivingMessages(user);
                        break;
                    }
                    string messageToClients = $"Client {user.Id} - {dataFromClient}";
                    SendMessageToAllClients(messageToClients);
                }
            }
            catch (Exception)
            {
                ChatFunctions.DisconnectClient(user);
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
                catch (Exception)
                {
                    ChatFunctions.ConsoleDisplayer.PrintValueToConsole($"CLient {client.Id} dissonnected");
                }
            }
        }

        public void EnterUserToChat(User user)
        {
            user.AddActiveChatId(_globalChatId);
            user.AddChat(GLOBAL_CHAT_NAME, _globalChatId, ChatOptions.Global);
            UsersInChat.Add(user);
            string clientConnectedMsg = $"Client {user.Id} connected";
            SendMessageToAllClients(clientConnectedMsg);
            ChatWithClient(user);
        }

        public void RemoveClientFromReceivingMessages(User user)
        {
            user.AddNumbChatId(_globalChatId);
            ChatFunctions.RemoveClient(user, _globalChatId);
            SendMessageToAllClients($"Client {user.Id} left chat");
        }
    }
}
