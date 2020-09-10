using BasicChatContents;
using ChatServer.ChatManagers;
using ChatServer.Chats;
using System;
using System.Collections.Generic;

namespace ChatServer.Handlers
{
    public class PrivateChatHandler
    {
        private List<User> _clients;
        private Dictionary<Guid, List<User>> _usersInChats;
        private List<Chat> _allChats;
        private GeneralChatFunctions _chatFunctions;
        private List<IChatManager> _allChatManagers;
        
        public PrivateChatHandler(List<User> clients, Dictionary<Guid, List<User>> usersInChats, List<Chat> allChats, 
            List<IChatManager> allChatManagers, ClientHandler clientHandler)
        {
            _clients = clients;
            _usersInChats = usersInChats;
            _chatFunctions = new GeneralChatFunctions(_usersInChats, _clients, clientHandler);
            _allChats = allChats;
            _allChatManagers = allChatManagers;
        }
        public void EnterUserToChat(User user, Guid id)
        {
            bool canSend = _chatFunctions.SendAllClientsConnected(user);
            if (canSend)
            {
                string clientId = _chatFunctions.GetDataFromClient(user);
                var secondUser = _chatFunctions.FindUser(clientId);
                if (!CheckIfUserAlreadyHasPrivateChat(user, secondUser))
                {
                    _usersInChats[id] = new List<User>() { user };
                    if (secondUser != null)
                    {
                        CreateNewChat(user, id, secondUser);
                    }
                }
            }
        }

        public void CreateNewChat(User user, Guid id, User secondUser)
        {
            Chat newPrivateChat = new Chat($"C{user.Id} + C{secondUser.Id}", id, ChatOptions.Private);
            _allChats.Add(newPrivateChat);
            user.AddChat(newPrivateChat);
            _usersInChats[id].Add(secondUser);
            PrivateChatManager privateChatManager = new PrivateChatManager(_chatFunctions, newPrivateChat);
            _allChatManagers.Add(privateChatManager);
            privateChatManager.OtherUsersInChat.Add(secondUser);
            _chatFunctions.AddChatToAllUsers(new List<User>() { secondUser }, newPrivateChat);
            privateChatManager.EnterUserToChat(user);
        }

        public bool CheckIfUserAlreadyHasPrivateChat(User user, User secondUser)
        {
            foreach (KeyValuePair<Guid, List<User>> chat in _usersInChats)
            {
                if (chat.Value.Count == 2)
                {
                    if (chat.Value.Contains(user) && chat.Value.Contains(secondUser))
                    {
                        foreach (var manager in _allChatManagers)
                        {
                            if (manager.OtherUsersInChat.Count == 1 &&
                                (manager.OtherUsersInChat[0] == user || manager.OtherUsersInChat[0] == secondUser))
                            {
                                manager.EnterUserToChat(user);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
