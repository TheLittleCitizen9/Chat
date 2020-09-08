using System;

namespace ChatClient.Chats
{
    public class PrivateChat : BasicChat
    {
        private const string NO_OTHER_USERS_CONNECTED = "No other users connected";
        public PrivateChat(Byte[] bytes, Client tcpClient, ConsoleDisplayer consoleDisplayer)
            : base(bytes, tcpClient, consoleDisplayer)
        {

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
            if(allClients[0].Replace("\0", string.Empty) != NO_OTHER_USERS_CONNECTED)
            {
                PrintClientsConnected(allClients);
                _consoleDisplayer.PrintValueToConsole("Enter client ID to talk to");
                string id = Console.ReadLine();
                if (ValidateId(id, allClients))
                {
                    SendServerClientId(id);
                }
                else
                {
                    _consoleDisplayer.PrintValueToConsole("Please enter a valid id");
                }
            }
            else
            {
                _consoleDisplayer.PrintValueToConsole("Please choose a different option - there are no other users connected");
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
