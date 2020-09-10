using BasicChatContents;
using ChatServer.Chats;
using System;
using System.Collections.Generic;

namespace ChatServer.ChatManagers
{
    public class PrivateChatManager : BasicChat, IChatManager
    {
        public List<User> OtherUsersInChat { get; set; }

        private Chat _chat;

        public PrivateChatManager(GeneralChatFunctions chatFunctions, Chat chat)
        {
            UsersInChat = new List<User>();
            ChatFunctions = chatFunctions;
            //ChatFunctions.ActiveUsersInChat = UsersInChat;
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
        public void EnterUserToChat(User user)
        {
            user.AddActiveChatId(_chat.Id);
            user.AddChat(_chat);
            UsersInChat.Add(user);
            string clientConnectedMsg = $"Client {user.Id} connected";
            SendMessageToClients(clientConnectedMsg);
            ChatWithClient(user);
        }

        public void RemoveClientFromReceivingMessages(User user)
        {
            user.AddNumbChatId(_chat.Id);
            ChatFunctions.RemoveClient(user, UsersInChat, _chat.Id);
            SendMessageToClients($"Client {user.Id} left chat");
        }
    }
}
