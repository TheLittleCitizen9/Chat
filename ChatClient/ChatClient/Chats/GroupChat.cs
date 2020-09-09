using System;
using System.Collections.Generic;
using System.Text;

namespace ChatClient.Chats
{
    public class GroupChat : BasicChat
    {
        private const string NO_OTHER_USERS_CONNECTED = "No other users connected";
        public GroupChat(Byte[] bytes, Client tcpClient, ConsoleDisplayer consoleDisplayer)
            : base(bytes, tcpClient, consoleDisplayer)
        {

        }

        public override void Run()
        {
            string allClientsConnected = _client.GetMessageFromServer();
            if (ChooseClientToTalkTo(allClientsConnected))
            {
                EnterChatName();
                base.ReadFromServer();
                ShowOptions();
                WriteMessage();
            }
        }

        public override void WriteMessage()
        {
            while (true)
            {
                _consoleDisplayer.PrintValueToConsole("Enter message to send");
                string message = Console.ReadLine();
                if (message == "return" || message == "leave")
                {
                    WriteMessage(message);
                    break;
                }
                WriteMessage(message);
            }
        }

        public bool ChooseClientToTalkTo(string allClientsConnected)
        {
            string[] allClients = allClientsConnected.Split(',');
            if (allClients[0].Replace("\0", string.Empty) != NO_OTHER_USERS_CONNECTED)
            {
                PrintClientsConnected(allClients);
                _consoleDisplayer.PrintValueToConsole("Enter client IDs to talk to (put ',' between IDs)");
                string ids = Console.ReadLine();
                string[] clientsToAdd = ids.Split(',');
                if (ValidateIds(clientsToAdd, allClients))
                {
                    SendServerClientIds(ids);
                    return true;
                }
                else
                {
                    _consoleDisplayer.PrintValueToConsole("Please enter valid ids");
                    return false;
                }
            }
            else
            {
                _consoleDisplayer.PrintValueToConsole("Please choose a different option - there are no other users connected");
                return false;
            }
        }

        private void EnterChatName()
        {
            _consoleDisplayer.PrintValueToConsole("Enter Chat Name");
            string chatName = Console.ReadLine();
            base.WriteMessage(chatName);
        }

        private void PrintClientsConnected(string[] clients)
        {
            foreach (var client in clients)
            {
                _consoleDisplayer.PrintValueToConsole(client);
            }
        }

        private void SendServerClientIds(string ids)
        {
            base.WriteMessage(ids);
        }

        private bool ValidateIds(string[] ids, string[] allClients)
        {
            bool result = false;
            foreach (var client in allClients)
            {
                foreach (var id in ids)
                {
                    if (client.Contains(id))
                        result = true;
                }
            }
            return result;
        }
    }
}
