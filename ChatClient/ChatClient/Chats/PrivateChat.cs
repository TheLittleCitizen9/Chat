using BasicChatContents;
using System;

namespace ChatClient.Chats
{
    public class PrivateChat : BasicChat
    {
        private const string NO_OTHER_USERS_CONNECTED = "No other users connected";
        private ChatUtils _chatUtils;
        public PrivateChat(Byte[] bytes, Client tcpClient, ConsoleDisplayer consoleDisplayer)
            : base(bytes, tcpClient, consoleDisplayer)
        {
            _chatUtils = new ChatUtils();
        }

        public override void Run()
        {
            string allClientsConnected = _client.GetMessageFromServer();
            if(ChooseClientToTalkTo(allClientsConnected))
            {
                base.ReadFromServer();
                ShowOptions();
                WriteMessage();
            }
        }

        public bool ChooseClientToTalkTo(string allClientsConnected)
        {
            string[] allClients = allClientsConnected.Split(',');
            if(allClients[0].Replace("\0", string.Empty) != NO_OTHER_USERS_CONNECTED)
            {
                _chatUtils.PrintClientsConnected(allClients);
                _consoleDisplayer.PrintValueToConsole("Enter client ID to talk to");
                string id = Console.ReadLine();
                if (_chatUtils.ValidateId(id, allClients))
                {
                    SendServerClientId(id);
                    return true;
                }
                else
                {
                    _consoleDisplayer.PrintValueToConsole("Please enter a valid id");
                    return false;
                }
            }
            else
            {
                _consoleDisplayer.PrintValueToConsole("Please choose a different option - there are no other users connected");
                return false;
            }
        }

        private void SendServerClientId(string id)
        {
            WriteMessage(id);
        }
    }
}
