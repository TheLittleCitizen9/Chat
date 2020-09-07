using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Chats
{
    public abstract class BasicChat
    {
        protected Byte[] _bytesReceived;
        protected TcpClient _client;
        protected ConsoleDisplayer _consoleDisplayer;

        public BasicChat(Byte[] bytes, TcpClient tcpClient, ConsoleDisplayer consoleDisplayer)
        {
            _bytesReceived = bytes;
            _client = tcpClient;
            _consoleDisplayer = consoleDisplayer;
        }

        public virtual void Run()
        {
            ReadFromServer();
        }

        public virtual void ShowOptions()
        {

        }
        public virtual void WriteMessage(string message)
        {
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
        public virtual void ReadFromServer()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        NetworkStream nwStream = _client.GetStream();
                        nwStream.Read(_bytesReceived, 0, _bytesReceived.Length);
                        _consoleDisplayer.PrintValueToConsole(Encoding.ASCII.GetString(_bytesReceived));
                    }
                    catch (IOException e)
                    {
                        _consoleDisplayer.PrintValueToConsole($"Server disconnected");
                        Exit();
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
