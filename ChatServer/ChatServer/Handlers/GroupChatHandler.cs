using ChatServer.ChatManagers;
using ChatServer.Chats;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatServer.Handlers
{
    public class GroupChatHandler
    {
        private List<User> _clients;
        private Dictionary<Guid, List<User>> _usersInChats;
        private List<Chat> _allChats;
        private GeneralChatFunctions _chatFunctions;
        private List<IChatManager> _allChatManagers;
        private GeneralHandler _generalHandler;


        public GroupChatHandler(List<User> clients, Dictionary<Guid, List<User>> usersInChats, List<Chat> allChats,
            List<IChatManager> allChatManagers, GeneralHandler generalHandler)
        {
            _clients = clients;
            _usersInChats = usersInChats;
            _chatFunctions = new GeneralChatFunctions(_usersInChats, _clients);
            _allChats = allChats;
            _allChatManagers = allChatManagers;
            _generalHandler = generalHandler;
        }

        public void EnterUserToChat(User user, Guid id)
        {
            bool canSend = _generalHandler.SendAllClientsConnected(user);
            if (canSend)
            {
                string clientIds = _chatFunctions.GetDataFromClient(user);
                var users = FindMultipleUsers(clientIds.Split(','));
                if (!CheckIfUserAlreadyHasGroupChat(user, users))
                {
                    _usersInChats[id] = new List<User>() { user };
                    if(users.Count > 0)
                    {
                        string chatName = _chatFunctions.GetDataFromClient(user);
                        CreateNewChat(user, id, users, chatName);
                    }
                }
            }
        }

        public void CreateNewChat(User user, Guid id, List<User> otherUsers, string chatName)
        {
            GroupChat newGroupChat = new GroupChat($"{chatName}", id, ChatOptions.Group);
            newGroupChat.Admins.Add(user);
            _allChats.Add(newGroupChat);
            _usersInChats[id].AddRange(otherUsers);
            GroupChatManager groupChatManager = new GroupChatManager(_chatFunctions, newGroupChat);
            _allChatManagers.Add(groupChatManager);
            groupChatManager.OtherUsersInChat.AddRange(otherUsers);
            groupChatManager.AddChatToAllUsers(otherUsers);
            groupChatManager.EnterUserToChat(user);
        }

        public bool CheckIfUserAlreadyHasGroupChat(User user, List<User> otherUsers)
        {
            var allUsersInChat = otherUsers.Select(u => u).ToList();
            allUsersInChat.Add(user);
            foreach (KeyValuePair<Guid, List<User>> chat in _usersInChats)
            {
                if (chat.Value.Count >= 2)
                {
                    var firstNotSecond = chat.Value.Except(allUsersInChat).ToList();
                    var secondNotFirst = allUsersInChat.Except(chat.Value).ToList();
                    if (!firstNotSecond.Any() && !secondNotFirst.Any())
                    {
                        foreach (var manager in _allChatManagers)
                        {
                            if (manager.OtherUsersInChat.Count == otherUsers.Count && 
                                (_generalHandler.CheckIfAListContainsAnother(otherUsers, manager.OtherUsersInChat) ||
                                _generalHandler.CheckIfAListContainsAnother(otherUsers, manager.UsersInChat)))
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

        private List<User> FindMultipleUsers(string[] userIds)
        {
            List<User> users = new List<User>();
            foreach (var id in userIds)
            {
                var user = FindUser(id);
                if (user != null)
                    users.Add(user);
            }
            return users;
        }

        private User FindUser(string id)
        {
            User user = null;
            int userId = int.Parse(id);
            foreach (var client in _clients)
            {
                if (client.Id == userId)
                {
                    user = client;
                }
            }
            return user;
        }
    }
}
