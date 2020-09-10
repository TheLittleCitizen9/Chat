using BasicChatContents;
using ChatServer.ChatManagers;
using ChatServer.Chats;
using System;
using System.Collections.Generic;

namespace ChatServer.Handlers
{
    public class GlobalChatHandler
    {
        private List<User> _clients;
        private Dictionary<Guid, List<User>> _usersInChats;
        public GeneralChatFunctions ChatFunctions;
        private List<IChatManager> _allChatManagers;
        private GlobalChatManager _globalChatManager;
        private readonly Guid _globalChatId = Guid.NewGuid();
        private const string GLOBAL_CHAT_NAME = "Global";

        public GlobalChatHandler(List<User> clients, Dictionary<Guid, List<User>> usersInChats,
            List<IChatManager> allChatManagers, ClientHandler clientHandler)
        {
            _clients = clients;
            _usersInChats = usersInChats;
            ChatFunctions = new GeneralChatFunctions(_usersInChats, _clients, clientHandler);
            _allChatManagers = allChatManagers;
            _globalChatManager = new GlobalChatManager(_globalChatId, ChatFunctions);
        }

        public void EnterUserToChat(User user)
        {
            user.AddActiveChatId(_globalChatId);
            user.AddChat(new Chat(GLOBAL_CHAT_NAME, _globalChatId, ChatOptions.Global));
            _globalChatManager.EnterUserToChat(user);
        }

        public void AddGlobalChat()
        {
            _usersInChats[_globalChatId] = _globalChatManager.UsersInChat;
            _allChatManagers.Add(_globalChatManager);
        }
    }
}
