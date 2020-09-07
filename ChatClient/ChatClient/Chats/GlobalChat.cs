using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace ChatClient.Chats
{
    public class GlobalChat : BasicChat
    {
        public GlobalChat(Byte[] bytes, TcpClient tcpClient, ConsoleDisplayer consoleDisplayer) 
            :base(bytes, tcpClient, consoleDisplayer)
        {

        }

        public override void ShowOptions()
        {
            Dictionary<string, string> options = new Dictionary<string, string>();
            options.Add("return", "return to main menu");
            options.Add("exit", "exit program");
            _consoleDisplayer.PrintMenu(options);
        }

        public void WriteMessage()
        {
            while (true)
            {
                _consoleDisplayer.PrintValueToConsole("Enter message to send");
                string message = Console.ReadLine();
                if(message == "return")
                {
                    base.WriteMessage(message);
                    break;
                }
                base.WriteMessage(message);
            }
        }
        public override void Run()
        {
            base.ReadFromServer();
            WriteMessage();
        }
    }
}
