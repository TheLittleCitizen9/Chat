﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ChatServer.ChatManagers
{
    public class GlobalChatManager
    {
        private Guid _globalChatId;
        public List<User> UsersInChat;
        private const string GLOBAL_CHAT_NAME = "Global";
        public GlobalChatFunctions ChatFunctions;

        public GlobalChatManager(Guid id, GlobalChatFunctions chatFunctions)
        {
            _globalChatId = id;
            UsersInChat = new List<User>();
            ChatFunctions = chatFunctions;
        }
        public void ChatWithClientInGlobalChat(User user)
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
                catch (Exception e)
                {
                    ChatFunctions.ConsoleDisplayer.PrintValueToConsole(e.Message);
                }
            }
        }

        public void EnterUserToGlobalChat(User user)
        {
            user.AddActiveChatId(_globalChatId);
            user.AddChat(GLOBAL_CHAT_NAME, _globalChatId, ChatOptions.Global);
            UsersInChat.Add(user);
            string clientConnectedMsg = $"Client {user.Id} connected";
            SendMessageToAllClients(clientConnectedMsg);
            ChatWithClientInGlobalChat(user);
        }

        private void RemoveClientFromReceivingMessages(User user)
        {
            user.AddNumbChatId(_globalChatId);
            ChatFunctions.RemoveClient(user, _globalChatId);
            SendMessageToAllClients($"Client {user.Id} left chat");
        }
    }
}
