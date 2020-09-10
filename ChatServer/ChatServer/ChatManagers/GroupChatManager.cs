using ChatServer.Chats;
using System;
using System.Collections.Generic;

namespace ChatServer.ChatManagers
{
    public class GroupChatManager : BasicChat, IChatManager
    {
        public List<User> OtherUsersInChat { get; set; }

        private GroupChat _chat;
        private const string RETURN_TO_MENU = "return";
        private const string ADD_ADMIN = "add admin";
        private const string LEAVE_GROUP = "leave";
        private const string REMOVE_USER = "remove";

        public GroupChatManager(GeneralChatFunctions chatFunctions, GroupChat chat)
        {
            ChatFunctions = chatFunctions;
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
                    if (noNullValuesData == RETURN_TO_MENU)
                    {
                        RemoveClientFromReceivingMessages(user, _chat.Id);
                        break;
                    }
                    else if(noNullValuesData == ADD_ADMIN)
                    {
                        AddUserAsAdmin(user);
                    }
                    else if (noNullValuesData == REMOVE_USER)
                    {
                        RemoveUserFromGroup(user);
                    }
                    else if (noNullValuesData == LEAVE_GROUP)
                    {
                        LeaveGroup(user);
                        break;
                    }
                    else
                    {
                        string messageToClients = $"Client {user.Id} - {dataFromClient}";
                        SendMessageToClients(messageToClients);
                    }
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
            UsersInChat.Add(user);
            string clientConnectedMsg = $"Client {user.Id} connected";
            SendMessageToClients(clientConnectedMsg);
            ChatWithClient(user);
        }
        public void AddUserAsAdmin(User user)
        {
            if (_chat.Admins.Contains(user))
            {
                var newAdmin = GetUser(user);
                _chat.Admins.Add(newAdmin);
                SendMessageToClients($"{newAdmin.Id} is now an admin");
            }
            else
            {
                ChatFunctions.ClientHandler.SendClientMessage("You are not an admin, so you can't perform this action", user);
            }
        }

        public void RemoveUserFromGroup(User user)
        {
            if (_chat.Admins.Contains(user))
            {
                var userToRemove = GetUser(user);
                RemoveChatFromUser(userToRemove);
                _chat.Admins.Remove(userToRemove);
                UsersInChat.Remove(userToRemove);
                ChatFunctions.RemoveClientFromSpecificChat(userToRemove, _chat.Id);
                RemoveClientFromReceivingMessages(userToRemove, _chat.Id);
                SendMessageToClients($"{userToRemove.Id} was removed from group");
            }
            else
            {
                ChatFunctions.ClientHandler.SendClientMessage("You are not an admin, so you can't perform this action", user);
            }
        }

        public void LeaveGroup(User user)
        {
            if (_chat.Admins.Contains(user))
            {
                _chat.Admins.Remove(user);
            }
            UsersInChat.Remove(user);
            RemoveChatFromUser(user);
            ChatFunctions.RemoveClientFromSpecificChat(user, _chat.Id);
            RemoveClientFromReceivingMessages(user, _chat.Id);
            SendMessageToClients($"{user.Id} left the group");
        }

        private User GetUser(User user)
        {
            ChatFunctions.SendAllClientsInChat(user, _chat.Id);
            string userId = ChatFunctions.GetDataFromClient(user);
            return ChatFunctions.FindUser(userId);
        }

        private void RemoveChatFromUser(User userToRemove)
        {
            userToRemove.AllChats.Remove(_chat);
            userToRemove.ActiveChatIds.Remove(_chat.Id);
            userToRemove.InactiveChatIds.Remove(_chat.Id);
        }
    }
}
