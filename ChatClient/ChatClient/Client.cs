using ChatClient.Chats;
using System;
using System.Net.Sockets;

namespace ChatClient
{
    public class Client
    {
        public string Ip;
        public int Port;
        public Byte[] BytesReceived;
        public TcpClient TcpClient;
        public ConsoleDisplayer ConsoleDisplayer;
        public BasicChat CurrentChat;

        public Client()
        {
            BytesReceived = new Byte[256];
            ConsoleDisplayer = new ConsoleDisplayer();
        }

        public void CreateClient()
        {
            GetServerDetails();
            ConnectToServer();
        }
        public void GetServerDetails()
        {
            ConsoleDisplayer.PrintValueToConsole("Enter IP");
            Ip = Console.ReadLine();

            ConsoleDisplayer.PrintValueToConsole("Enter PORT");
            Port = int.Parse(Console.ReadLine());

        }
        public void ConnectToServer()
        {
            TcpClient = new TcpClient(Ip, Port);
        }
    }
}
