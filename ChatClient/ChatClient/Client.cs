using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient
{
    public class Client
    {
        private string _ip;
        private int _port;
        private Byte[] _bytesReceived;
        private TcpClient _client;
        private IPEndPoint _ipe;
        private ConsoleDisplayer _consoleDisplayer;

        public Client()
        {
            _bytesReceived = new Byte[256];
            _consoleDisplayer = new ConsoleDisplayer();
        }

        public void RunClient()
        {
            var ipa = GetServerDetails();
            ConnectToServer(ipa);
            ReadFromServer();
            WriteMessage();
        }
        public IPAddress GetServerDetails()
        {
            _consoleDisplayer.PrintValueToConsole("Enter IP");
            _ip = Console.ReadLine();
            IPAddress ipa = IPAddress.Parse(_ip);

            _consoleDisplayer.PrintValueToConsole("Enter PORT");
            _port = int.Parse(Console.ReadLine());

            return ipa;
        }
        public void ConnectToServer(IPAddress ipa)
        {
            _ipe = new IPEndPoint(ipa, _port);

            _client = new TcpClient(_ip, _port);
        }
        public void WriteMessage()
        {
            while(true)
            {
                _consoleDisplayer.PrintValueToConsole("Enter message to send");
                string message = Console.ReadLine();
                if (message == "exit")
                {
                    Exit();
                }
                else
                {
                    NetworkStream nwStream = _client.GetStream();
                    byte[] messageToSend = Encoding.ASCII.GetBytes(message);
                    nwStream.Write(messageToSend);
                }
            }
            
        }
        public void ReadFromServer()
        {
            Task.Run(() =>
            {
                while(true)
                {
                    try
                    {
                        NetworkStream nwStream = _client.GetStream();
                        nwStream.Read(_bytesReceived, 0, _bytesReceived.Length);
                        _consoleDisplayer.PrintValueToConsole(Encoding.ASCII.GetString(_bytesReceived));
                    }
                    catch (IOException e)
                    {
                        _consoleDisplayer.PrintValueToConsole($"IOException reading from socket: {e.Message}");
                    }
                }
            });
            
        }

        private void Exit()
        {
            _client.Close();
            Environment.Exit(0);
        }
    }
}
