using ChatServer.ChatManagers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ChatServer
{
    public class Server
    {
        private List<User> _clients;
        private int _port;
        private TcpListener _server;
        private const string IP = "10.1.0.17";
        private ConsoleDisplayer _consoleDisplayer;
        private int _counter = 0;
        private Guid _globalChatId = Guid.NewGuid();
        private Dictionary<Guid, List<User>> _usersInChats;
        private List<Chat> _allChats;
        private GlobalChatManager _globalChatManager;
        private GlobalChatFunctions _chatFunctions;
        private object _lock = new object();
        private List<IChatManager> _allChatManagers;
        private ClientHandler _clientHandler;

        public Server()
        {
            _clients = new List<User>();
            _consoleDisplayer = new ConsoleDisplayer();
            _usersInChats = new Dictionary<Guid, List<User>>();
            _chatFunctions = new GlobalChatFunctions(_usersInChats, _clients);
            _globalChatManager = new GlobalChatManager(_globalChatId, _chatFunctions);
            _allChats = new List<Chat>();
            _allChatManagers = new List<IChatManager>();
            _clientHandler = new ClientHandler();
        }

        public void StartListening()
        {
            CreateServerSocket();
            _usersInChats[_globalChatId] = _globalChatManager.UsersInChat;
            while (true)
            {
                var clientSocket = _server.AcceptTcpClient();
                _counter++;
                var user = new User(clientSocket, _counter);
                _consoleDisplayer.PrintValueToConsole($"Client {user.Id} connected");
                _clients.Add(user);
                Task task = new Task(() => MapCientsChatChoice(user));
                task.Start();
            }
        }

        private void MapCientsChatChoice(User user)
        {
            try
            {
                while (true)
                {
                    string clientChatChoice = _globalChatManager.ChatFunctions.GetDataFromClient(user);
                    object chatChoice;
                    if (Enum.TryParse(typeof(ChatOptions), clientChatChoice, out chatChoice))
                    {
                        if ((ChatOptions)chatChoice == ChatOptions.Global)
                        {
                            EnterGlobalChat(user);
                        }
                        else if ((ChatOptions)chatChoice == ChatOptions.Private)
                        {
                            Guid chatId = Guid.NewGuid();
                            EnterPrivateChat(user, chatId);
                        }
                        else if ((ChatOptions)chatChoice == ChatOptions.SeeAll)
                        {
                            _clientHandler.SendClientAllHisChats(user);
                        }
                    }
                }
            }
            catch (Exception e)
            {

                _consoleDisplayer.PrintValueToConsole(e.Message);
            }
        }

        private void EnterGlobalChat(User user)
        {
            if (user.NumbChatIds.Contains(_globalChatId))
            {
                _globalChatManager.EnterUserToGlobalChat(user);
            }
            else
            {
                //lock(_lock)
                //{
                //    _clients.Add(user);
                //}
                _globalChatManager.EnterUserToGlobalChat(user);
            }
        }
        private void EnterUserToChat(User user, Guid id)
        {
            bool canSend = SendAllClientsConnected(user);
            if (canSend)
            {
                string clientId = _chatFunctions.GetDataFromClient(user);
                var secondUser = FindUser(clientId);
                if(!CheckIfUserAlreadyHasPrivateChat(user, secondUser))
                {
                    _usersInChats[id] = new List<User>() { user };
                    if (secondUser != null)
                    {
                        Chat newPrivateChat = new Chat($"C{user.Id} + C{secondUser.Id}", id, ChatOptions.Private);
                        _allChats.Add(newPrivateChat);
                        _usersInChats[id].Add(secondUser);
                        PrivateChatManager privateChatManager = new PrivateChatManager(_chatFunctions, newPrivateChat);
                        _allChatManagers.Add(privateChatManager);
                        privateChatManager.SecondUser = secondUser;
                        privateChatManager.AddChatToAllUsers(new List<User>() { secondUser });
                        privateChatManager.EnterUserToChat(user);
                    }
                }
            }
        }

        private bool CheckIfUserAlreadyHasPrivateChat(User user, User secondUser)
        {
            foreach (KeyValuePair<Guid, List<User>> chat in _usersInChats)
            {
                if (chat.Value.Count == 2)
                {
                    if (chat.Value.Contains(user) && chat.Value.Contains(secondUser))
                    {
                        foreach (var manager in _allChatManagers)
                        {
                            if (manager.SecondUser == user)
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
        private void EnterPrivateChat(User user, Guid id)
        {
            if(user.NumbChatIds.Contains(id))
            {
                EnterUserToChat(user, id);
            }
            else
            {
                //lock(_lock)
                //{
                //    _clients.Add(user);
                //}
                EnterUserToChat(user, id);
            }
            
        }

        private User FindUser(string id)
        {
            User user = null;
            int userId = int.Parse(id);
            foreach (var client in _clients)
            {
                if(client.Id == userId)
                {
                    user = client;
                }
            }
            return user;
        }

        private bool SendAllClientsConnected(User user)
        {
            string noConnectedClients = "No other users connected";
            string allConnectedClients = string.Empty;
            lock(_lock)
            {
                foreach (var client in _clients)
                {
                    if (client != user)
                    {
                        allConnectedClients += $"Client {client.Id},";
                    }
                }
            }
            if(string.IsNullOrEmpty(allConnectedClients))
            {
                _clientHandler.SendClientMessage(noConnectedClients, user);
                return false;
            }
            else
            {
                _clientHandler.SendClientMessage(allConnectedClients, user);
                return true;
            }
        }

        private void CreateServerSocket()
        {
            IPAddress ipa = IPAddress.Parse(IP);
            _consoleDisplayer.PrintValueToConsole("Enter PORT");
            _port = int.Parse(Console.ReadLine());

            _server = new TcpListener(ipa, _port);
            _server.Start(100);
            _consoleDisplayer.PrintValueToConsole("Server started");
        }
    }
}
