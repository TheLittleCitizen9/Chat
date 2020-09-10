using BasicChatContents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatClient.Chats
{
    public class BasicChat
    {
        protected Byte[] _bytesReceived;
        protected Client _client;
        protected ConsoleDisplayer _consoleDisplayer;
        //protected CancellationTokenSource cancelationToken = new CancellationTokenSource();
        protected Dictionary<string, string> _options;

        public BasicChat(Byte[] bytes, Client tcpClient, ConsoleDisplayer consoleDisplayer)
        {
            _bytesReceived = bytes;
            _client = tcpClient;
            _consoleDisplayer = consoleDisplayer;
            _options = new Dictionary<string, string>();
        }

        public virtual void Run()
        {
            ReadFromServer();
        }

        public virtual void ShowOptions()
        {
            _options.Add("return", "return to main menu");
            _options.Add("exit", "exit program");
            _consoleDisplayer.PrintMenu(_options);
        }
        public void WriteMessage(string message)
        {
            if (message == "exit")
            {
                Exit();
            }
            else
            {
                NetworkStream nwStream = _client.TcpClient.GetStream();
                byte[] messageToSend = Encoding.ASCII.GetBytes(message);
                nwStream.Write(messageToSend);
            }
        }

        public virtual void WriteMessage()
        {
            while (true)
            {
                _consoleDisplayer.PrintValueToConsole("Enter message to send");
                string message = Console.ReadLine();
                if (message == "return")
                {
                    WriteMessage(message);
                    break;
                }
                WriteMessage(message);
            }
        }

        public void GetMessagesFromServer()
        {
            while (true)
            {
                try
                {
                    if (_client.TcpClient.GetStream().DataAvailable)
                    {
                        Thread.Sleep(15);
                        string recievedData = GetDataFromServer();

                        if (recievedData.Replace("\0", string.Empty) != string.Empty)
                        {
                            Thread processData = new Thread(() => PrintMessage(Encoding.ASCII.GetString(_bytesReceived)));
                            processData.Start();
                        }
                    }
                }
                catch (Exception e)
                {
                    _consoleDisplayer.PrintValueToConsole(e.Message);
                    _consoleDisplayer.PrintValueToConsole("Server disconnected");
                    Exit();
                }

                Thread.Sleep(5);
            }
        }

        //public virtual void GetMessagesFromServer()
        //{
        //    while (true)
        //    {
        //        try
        //        {
        //            if (_client.TcpClient.GetStream().DataAvailable)
        //            {
        //                NetworkStream nwStream = _client.TcpClient.GetStream();
        //                nwStream.Read(_bytesReceived, 0, _bytesReceived.Length);
        //                string recieved = Encoding.ASCII.GetString(_bytesReceived);
        //                if (recieved.Replace("\0", string.Empty) != string.Empty)
        //                {
        //                    _consoleDisplayer.PrintValueToConsole(Encoding.ASCII.GetString(_bytesReceived));
        //                }
        //            }
        //        }
        //        catch (IOException)
        //        {
        //            _consoleDisplayer.PrintValueToConsole($"Server disconnected");
        //            Exit();
        //        }
        //    }
        //}

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
            _client.TcpClient.Close();
            Environment.Exit(0);
        }

        private string GetDataFromServer()
        {
            NetworkStream nwStream = _client.TcpClient.GetStream();
            nwStream.Read(_bytesReceived, 0, _bytesReceived.Length);
            return Encoding.ASCII.GetString(_bytesReceived);
        }
    }
}
