using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatClient.Chats
{
    public abstract class BasicChat
    {
        protected Byte[] _bytesReceived;
        protected TcpClient _client;
        protected ConsoleDisplayer _consoleDisplayer;
        protected CancellationTokenSource cancelationToken = new CancellationTokenSource();

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

        public void GetMessagesFromServer()
        {
            while (true)
            {
                try
                {
                    if (_client.GetStream().DataAvailable)
                    {
                        Thread.Sleep(15);
                        string recievedData = GetDataFromClient();

                        if (recievedData.Replace("\0", string.Empty) != string.Empty)
                        {
                            Thread processData = new Thread(() => PrintMessage(Encoding.ASCII.GetString(_bytesReceived)));
                            processData.Start();
                        }
                    }
                }
                catch (IOException)
                {
                    _consoleDisplayer.PrintValueToConsole("Server disconnected");
                    Exit();
                }

                Thread.Sleep(5);
            }
        }

        public void PrintMessage(string message)
        {
            _consoleDisplayer.PrintValueToConsole(message);
        }
        

        public virtual void ReadFromServer()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                GetMessagesFromServer();
            }).Start();
        }

        public void Exit()
        {
            _client.Close();
            Environment.Exit(0);
        }

        private string GetDataFromClient()
        {
            NetworkStream nwStream = _client.GetStream();
            nwStream.Read(_bytesReceived, 0, _bytesReceived.Length);
            return Encoding.ASCII.GetString(_bytesReceived);
        }
    }
}
