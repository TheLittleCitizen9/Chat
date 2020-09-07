using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

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

            while (true)
            {
                ConnectToServer(ipa);
            }
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
            try
            {
                string str;

                while ((str = Console.ReadLine()) != "")
                {
                    BinaryWriter writer = new BinaryWriter(_client.GetStream());
                    _consoleDisplayer.PrintValueToConsole(str);
                }
            }
            catch (IOException e)
            {
                _consoleDisplayer.PrintValueToConsole($"IOException writing to socket: {e.Message}");
            }
        }

        public void ReadFromServer()
        {
            try
            {
                NetworkStream nwStream = _client.GetStream();
                nwStream.Read(_bytesReceived, 0, _bytesReceived.Length);
                _consoleDisplayer.PrintValueToConsole(Encoding.ASCII.GetString(_bytesReceived));
                _client.Close();
            }
            catch (IOException e)
            {
                _consoleDisplayer.PrintValueToConsole($"IOException reading from socket: {e.Message}");
            }
        }
    }
}
