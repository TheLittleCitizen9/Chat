using ChatClient.Chats;
using System;
using System.Net.Sockets;
using System.Text;

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

        public void SendMessageToServer(string message)
        {
            NetworkStream nwStream = TcpClient.GetStream();
            byte[] messageToSend = Encoding.ASCII.GetBytes(message);
            nwStream.Write(messageToSend);
        }

        public string GetMessageFromServer()
        {
            NetworkStream nwStream = TcpClient.GetStream();
            nwStream.Read(BytesReceived, 0, BytesReceived.Length);
            return Encoding.ASCII.GetString(BytesReceived);
        }
    }
}
