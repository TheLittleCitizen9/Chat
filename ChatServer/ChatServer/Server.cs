using ChatServer.ChatManagers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
        private GlobalChatManager _globalChatManager;

        public Server()
        {
            _clients = new List<User>();
            _consoleDisplayer = new ConsoleDisplayer();
            _usersInChats = new Dictionary<Guid, List<User>>();
            _globalChatManager = new GlobalChatManager(_globalChatId, _usersInChats, _clients);
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
                    string clientChatChoice = _globalChatManager.GetDataFromClient(user);
                    object chatChoice;
                    if (Enum.TryParse(typeof(ChatOptions), clientChatChoice, out chatChoice))
                    {
                        if ((ChatOptions)chatChoice == ChatOptions.Global)
                        {
                            EnterGlobalChat(user);
                        }
                        else if ((ChatOptions)chatChoice == ChatOptions.Private)
                        {
                            //EnterGlobalChat(user);
                        }
                        else if ((ChatOptions)chatChoice == ChatOptions.SeeAll)
                        {
                            SendClientAllHisChats(user);
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
                _clients.Add(user);
                _globalChatManager.EnterUserToGlobalChat(user);
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

        private void SendClientAllHisChats(User user)
        {
            string allChatsOfClient = string.Empty;
            foreach (var chat in user.AllChats)
            {
                allChatsOfClient += $"{chat.Name}-{chat.ChatOption},";
            }
            byte[] data = Encoding.ASCII.GetBytes(allChatsOfClient);
            NetworkStream nwStream = user.ClientSocket.GetStream();
            nwStream.Write(data);
        }
    }
}
