using ChatServer.Chats;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ChatServer.ChatManagers
{
    public class PrivateChatManager : IChatManager
    {
        public List<User> UsersInChat { get; set; }
        public GeneralChatFunctions ChatFunctions;
        public List<User> OtherUsersInChat { get; set; }

        private Chat _chat;

        public PrivateChatManager(GeneralChatFunctions chatFunctions, Chat chat)
        {
            UsersInChat = new List<User>();
            ChatFunctions = chatFunctions;
            ChatFunctions.ActiveUsersInChat = UsersInChat;
            _chat = chat;
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
                    SendMessageToClients(messageToClients);
                }
            }
            catch (Exception)
            {
                ChatFunctions.DisconnectClient(user);
            }
        }
        public void SendMessageToClients(string dataToSend)
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
                    ChatFunctions.ConsoleDisplayer.PrintValueToConsole(e.Message);
                }
            }
        }

        public void EnterUserToChat(User user)
        {
            user.AddActiveChatId(_chat.Id);
            user.AddChat(_chat.Name, _chat.Id, ChatOptions.Private);
            UsersInChat.Add(user);
            string clientConnectedMsg = $"Client {user.Id} connected";
            SendMessageToClients(clientConnectedMsg);
            ChatWithClient(user);
        }

        public void RemoveClientFromReceivingMessages(User user)
        {
            user.AddNumbChatId(_chat.Id);
            ChatFunctions.RemoveClient(user, _chat.Id);
            SendMessageToClients($"Client {user.Id} left chat");
        }

        public void AddChatToAllUsers(List<User> users)
        {
            foreach (var user in users)
            {
                user.NumbChatIds.Add(_chat.Id);
                user.AllChats.Add(_chat);
            }
        }
    }
}
