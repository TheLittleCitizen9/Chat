using ChatServer.Chats;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ChatServer.ChatManagers
{
    public class GroupChatManager : IChatManager
    {
        public List<User> UsersInChat { get; set; }
        public GeneralChatFunctions ChatFunctions;
        public List<User> OtherUsersInChat { get; set; }

        private GroupChat _chat;
        private const string RETURN_TO_MENU = "return";
        private const string ADD_ADMIN = "add admin";
        private const string LEAVE_GROUP = "leave";
        private const string REMOVE_USER = "remove";

        public GroupChatManager(GeneralChatFunctions chatFunctions, GroupChat chat)
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
                    if (noNullValuesData == RETURN_TO_MENU)
                    {
                        RemoveClientFromReceivingMessages(user);
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
            user.AddChat(_chat.Name, _chat.Id, ChatOptions.Group);
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

        public void AddUserAsAdmin(User user)
        {
            if (_chat.Admins.Contains(user))
            {
                ChatFunctions.SendAllClientsConnected(user);
                string userId = ChatFunctions.GetDataFromClient(user);
                var newAdmin = ChatFunctions.FindUser(userId);
                _chat.Admins.Add(newAdmin);
                SendMessageToClients($"{newAdmin.Id} is now an admin");
            }
        }

        public void RemoveUserFromGroup(User user)
        {
            if (_chat.Admins.Contains(user))
            {
                ChatFunctions.SendAllClientsConnected(user);
                string userId = ChatFunctions.GetDataFromClient(user);
                var userToRemove = ChatFunctions.FindUser(userId);
                userToRemove.AllChats.Remove(_chat);
                _chat.Admins.Remove(userToRemove);
                ChatFunctions.RemoveClientFromSpecificChat(userToRemove, _chat.Id);
                RemoveClientFromReceivingMessages(userToRemove);
                SendMessageToClients($"{userToRemove.Id} was removed from group");
            }
        }

        public void LeaveGroup(User user)
        {
            if (_chat.Admins.Contains(user))
            {
                _chat.Admins.Remove(user);
            }
            UsersInChat.Remove(user);
            ChatFunctions.RemoveClientFromSpecificChat(user, _chat.Id);
            RemoveClientFromReceivingMessages(user);
            SendMessageToClients($"{user.Id} left the group");
        }
    }
}
