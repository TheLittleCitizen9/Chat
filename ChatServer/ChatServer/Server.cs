using BasicChatContents;
using ChatServer.ChatManagers;
using ChatServer.Chats;
using ChatServer.Handlers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ChatServer
{
    public class Server
    {
        private const string IP = "10.1.0.17";
        private List<User> _clients;
        private int _port;
        private TcpListener _server;
        private ConsoleDisplayer _consoleDisplayer;
        private Dictionary<Guid, List<User>> _usersInChats;
        private List<Chat> _allChats;
        private GlobalChatHandler _globalChatHandler;
        private List<IChatManager> _allChatManagers;
        private ClientHandler _clientHandler;
        private PrivateChatHandler _privateChatHandler;
        private GroupChatHandler _groupChatHandler;

        public Server()
        {
            _clientHandler = new ClientHandler();
            _clients = new List<User>();
            _consoleDisplayer = new ConsoleDisplayer();
            _usersInChats = new Dictionary<Guid, List<User>>();
            _allChatManagers = new List<IChatManager>();
            _allChats = new List<Chat>();
            _globalChatHandler = new GlobalChatHandler(_clients, _usersInChats, _allChatManagers, _clientHandler);
            _privateChatHandler = new PrivateChatHandler(_clients, _usersInChats, _allChats, _allChatManagers,  _clientHandler);
            _groupChatHandler = new GroupChatHandler(_clients, _usersInChats, _allChats, _allChatManagers, _clientHandler);
        }

        public void StartListening()
        {
            int usersCounter = 0;
            CreateServerSocket();
            _globalChatHandler.AddGlobalChat();
            while (true)
            {
                var clientSocket = _server.AcceptTcpClient();
                usersCounter++;
                var user = new User(clientSocket, usersCounter);
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
                    string clientChatChoice = _globalChatHandler.ChatFunctions.GetDataFromClient(user);
                    if (Enum.TryParse(typeof(ChatOptions), clientChatChoice, out object chatChoice))
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
                        else if ((ChatOptions)chatChoice == ChatOptions.Group)
                        {
                            Guid chatId = Guid.NewGuid();
                            EnterGroupChat(user, chatId);
                        }
                    }
                }
            }
            catch (Exception)
            {
                _clients.Remove(user);
                _consoleDisplayer.PrintValueToConsole($"Client {user.Id} disconnected");
            }
        }

        private void EnterGlobalChat(User user)
        {
            _globalChatHandler.EnterUserToChat(user);
        }
        
        private void EnterPrivateChat(User user, Guid id)
        {
            _privateChatHandler.EnterUserToChat(user, id);
        }

        private void EnterGroupChat(User user, Guid id)
        {
            _groupChatHandler.EnterUserToChat(user, id);
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