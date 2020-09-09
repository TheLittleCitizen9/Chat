using ChatServer.ChatManagers;
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
        private int _counter = 0;
        private Guid _globalChatId = Guid.NewGuid();
        private Dictionary<Guid, List<User>> _usersInChats;
        private List<Chat> _allChats;
        private GlobalChatManager _globalChatManager;
        private GeneralChatFunctions _chatFunctions;
        private List<IChatManager> _allChatManagers;
        private ClientHandler _clientHandler;
        private PrivateChatHandler _privateChatHandler;
        private GeneralHandler _generalHandler;

        public Server()
        {
            _clients = new List<User>();
            _consoleDisplayer = new ConsoleDisplayer();
            _usersInChats = new Dictionary<Guid, List<User>>();
            _chatFunctions = new GeneralChatFunctions(_usersInChats, _clients);
            _globalChatManager = new GlobalChatManager(_globalChatId, _chatFunctions);
            _allChats = new List<Chat>();
            _allChatManagers = new List<IChatManager>();
            _clientHandler = new ClientHandler();
            _generalHandler = new GeneralHandler(_clients, _clientHandler);
            _privateChatHandler = new PrivateChatHandler(_clients, _usersInChats, _allChats, _allChatManagers, _generalHandler);
        }

        public void StartListening()
        {
            CreateServerSocket();
            _usersInChats[_globalChatId] = _globalChatManager.UsersInChat;
            _allChatManagers.Add(_globalChatManager);
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
            _globalChatManager.OtherUsersInChat.Add(user);
            _globalChatManager.EnterUserToChat(user);
        }
        
        private void EnterPrivateChat(User user, Guid id)
        {
            _privateChatHandler.EnterUserToPrivateChat(user, id);
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