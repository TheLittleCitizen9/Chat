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
        private ConcurrentDictionary<TcpClient, string> _clients;
        private int _port;
        private TcpListener _server;
        private const string IP = "10.1.0.17";
        private ConsoleDisplayer _consoleDisplayer;

        public Server()
        {
            _clients = new ConcurrentDictionary<TcpClient, string>();
            _consoleDisplayer = new ConsoleDisplayer();
        }

        public void CreateServerSocket()
        {
            IPAddress ipa = IPAddress.Parse(IP);
            _consoleDisplayer.PrintValueToConsole("Enter PORT");
            _port = int.Parse(Console.ReadLine());
            IPEndPoint ipe = new IPEndPoint(ipa, _port);

            _server = new TcpListener(ipa, _port);

            _server.Start(100);

            _consoleDisplayer.PrintValueToConsole("Server started");

            while (true)
            {
                var clientSocket = _server.AcceptTcpClient();
                Task task = new Task(() => Chat(clientSocket));
                task.Start();
            }
        }

        private void Chat(TcpClient clientSocket)
        {
            BinaryReader reader = new BinaryReader(clientSocket.GetStream());

            try
            {
                while (true)
                {
                    string message = reader.ReadString();
                    foreach (var client in _clients.Keys)
                    {
                        BinaryWriter writer = new BinaryWriter(client.GetStream());
                        writer.Write(message);
                    }
                }
            }
            catch (EndOfStreamException)
            {
                _consoleDisplayer.PrintValueToConsole($"client disconnecting: {clientSocket.Client.RemoteEndPoint}");
                clientSocket.Client.Shutdown(SocketShutdown.Both);
            }
            catch (IOException e)
            {
                _consoleDisplayer.PrintValueToConsole($"IOException reading from {clientSocket.Client.RemoteEndPoint}: {e.Message}");
            }

            clientSocket.Close();
            RemoveClient(clientSocket);
            _consoleDisplayer.PrintValueToConsole($"{GetClientsCount()} clients connected");
        }

        private int GetClientsCount()
        {
            return _clients.Count;
        }

        private void RemoveClient(TcpClient client)
        {
            string returnedValue;
            _clients.TryRemove(client, out returnedValue);
        }
    }
}
