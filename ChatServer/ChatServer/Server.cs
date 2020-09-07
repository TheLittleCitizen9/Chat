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

        public Server()
        {
            _clients = new List<User>();
            _consoleDisplayer = new ConsoleDisplayer();
        }

        public void StartListening()
        {
            CreateServerSocket();
            while (true)
            {
                var clientSocket = _server.AcceptTcpClient();
                _counter++;
                var user = new User(clientSocket, _counter);
                _clients.Add(user);
                string clientConnectedMsg = $"Client {user.Id} connected";
                SendToClients(clientConnectedMsg);
                Task task = new Task(() => ChatWithClient(user));
                task.Start();
            }
        }

        public void ChatWithClient(User user)
        {
            try
            {
                while (true)
                {
                    //check what kind of chat the user wants
                    byte[] buffer = new byte[1024];
                    NetworkStream nwStream = user.ClientSocket.GetStream();
                    int bytesRecieved = nwStream.Read(buffer);
                    string stringData = Encoding.ASCII.GetString(buffer);
                    //need to check the stringData for return value
                    string messageToClients = $"Client {user.Id} - {stringData}";
                    SendToClients(messageToClients);
                }
            }
            catch (Exception e)
            {
                DisconnectClient(user);
            }
        }

        public void SendToClients(string dataToSend)
        {
            byte[] data = Encoding.ASCII.GetBytes(dataToSend);
            foreach (var client in _clients)
            {
                try
                {
                    NetworkStream nwStream = client.ClientSocket.GetStream();
                    nwStream.Write(data);
                }
                catch (Exception e)
                {
                    _consoleDisplayer.PrintValueToConsole(e.Message);
                }
            }
        }

        private void DisconnectClient(User user)
        {
            string clientDisconnectedMsg = $"Client {user.ClientSocket.Client.RemoteEndPoint} disconnected";
            _consoleDisplayer.PrintValueToConsole(clientDisconnectedMsg);
            RemoveClient(user);
            SendToClients(clientDisconnectedMsg);
        }

        private void RemoveClient(User user)
        {
            _clients.Remove(user);
        }

        private void CreateServerSocket()
        {
            IPAddress ipa = IPAddress.Parse(IP);
            _consoleDisplayer.PrintValueToConsole("Enter PORT");
            _port = int.Parse(Console.ReadLine());
            IPEndPoint ipe = new IPEndPoint(ipa, _port);

            _server = new TcpListener(ipa, _port);

            _server.Start(100);

            _consoleDisplayer.PrintValueToConsole("Server started");
        }
    }
}
