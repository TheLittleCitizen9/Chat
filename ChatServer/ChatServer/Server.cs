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
        private List<TcpClient> _clients;
        private int _port;
        private TcpListener _server;
        private const string IP = "10.1.0.17";
        private ConsoleDisplayer _consoleDisplayer;

        public Server()
        {
            _clients = new List<TcpClient>();
            _consoleDisplayer = new ConsoleDisplayer();
        }

        public void StartListening()
        {
            CreateServerSocket();
            while (true)
            {
                var clientSocket = _server.AcceptTcpClient();
                _clients.Add(clientSocket);
                Task task = new Task(() => ChatWithClient(clientSocket));
                task.Start();
            }
        }

        public void ChatWithClient(TcpClient clientSocket)
        {
            try
            {
                while (true)
                {
                    byte[] buffer = new byte[1024];
                    NetworkStream nwStream = clientSocket.GetStream();
                    int bytesRecieved = nwStream.Read(buffer);
                    string stringData = Encoding.ASCII.GetString(buffer);
                    string messageToClients = $"Client {clientSocket.Client.RemoteEndPoint} - {stringData}";
                    SendToClients(messageToClients);
                }
            }
            catch (Exception e)
            {
                RemoveClient(clientSocket);
                _consoleDisplayer.PrintValueToConsole($"Client {clientSocket.Client.RemoteEndPoint} disconnected");
            }
        }

        public void SendToClients(string dataToSend)
        {
            byte[] data = Encoding.ASCII.GetBytes(dataToSend);
            foreach (var client in _clients)
            {
                NetworkStream nwStream = client.GetStream();
                nwStream.Write(data);
            }
        }

        private void RemoveClient(TcpClient client)
        {
            _clients.Remove(client);
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
