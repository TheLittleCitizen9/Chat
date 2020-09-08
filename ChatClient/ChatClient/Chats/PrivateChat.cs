using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ChatClient.Chats
{
    public class PrivateChat : BasicChat
    {
        public PrivateChat(Byte[] bytes, Client tcpClient, ConsoleDisplayer consoleDisplayer)
            : base(bytes, tcpClient, consoleDisplayer)
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
                if (message == "return")
                {
                    base.WriteMessage(message);
                    break;
                }
                base.WriteMessage(message);
            }
        }

        public override void Run()
        {
            string allClientsConnected = _client.GetMessageFromServer();
            ChooseClientToTalkTo(allClientsConnected);
            base.ReadFromServer();
            WriteMessage();
        }

        public void ChooseClientToTalkTo(string allClientsConnected)
        {
            string[] allClients = allClientsConnected.Split(',');
            PrintClientsConnected(allClients);
            _consoleDisplayer.PrintValueToConsole("Enter client ID to talk to");
            string id = Console.ReadLine();
            if(ValidateId(id, allClients))
            {
                SendServerClientId(id);
            }
            else
            {
                _consoleDisplayer.PrintValueToConsole("Please enter a valid id");
            }
        }

        private void PrintClientsConnected(string[] clients)
        {
            foreach (var client in clients)
            {
                _consoleDisplayer.PrintValueToConsole(client);
            }
        }

        private void SendServerClientId(string id)
        {
            base.WriteMessage(id);
        }

        private bool ValidateId(string id, string[] allClients)
        {
            foreach (var client in allClients)
            {
                if (client.Contains(id))
                    return true;
            }
            return false;
        }
    }
}
