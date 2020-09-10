using BasicChatContents;
using System;
using System.Collections.Generic;

namespace ChatServer.ChatManagers
{
    public class GlobalChatManager : BasicChat, IChatManager
    {
        private Guid _globalChatId;
        private const string GLOBAL_CHAT_NAME = "Global";
        public List<User> OtherUsersInChat { get; set; }

        public GlobalChatManager(Guid id, GeneralChatFunctions chatFunctions):base()
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
                        RemoveClientFromReceivingMessages(user, _globalChatId);
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
            user.AddActiveChatId(_globalChatId);
            user.AddChat(new Chats.Chat(GLOBAL_CHAT_NAME, _globalChatId, ChatOptions.Global));
            UsersInChat.Add(user);
            string clientConnectedMsg = $"Client {user.Id} connected";
            SendMessageToClients(clientConnectedMsg);
            ChatWithClient(user);
        }
    }
}
