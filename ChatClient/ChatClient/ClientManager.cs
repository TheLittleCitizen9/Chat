using BasicChatContents;
using ChatClient.Chats;
using System;
using System.Collections.Generic;

namespace ChatClient
{
    public class ClientManager
    {
        private const string GET_ALL_CHATS = "0";
        private const string REGISTER_TO_GLOBAL_CHAT = "1";
        private const string REGISTER_TO_PRIVATE_CHAT = "2";
        private const string CREATE_GROUP_CHAT = "3";
        private Dictionary<string, string> _dispalyOptions;
        private ConsoleDisplayer _consoleDisplayer;
        private Client _client;

        public ClientManager(Dictionary<string, string> options)
        {
            _dispalyOptions = options;
            _consoleDisplayer = new ConsoleDisplayer();
            _client = new Client();
        }

        public void InitializeClient()
        {
            _client.CreateClient();
        }

        public void NavigateToChoice()
        {
            while (true)
            {
                _consoleDisplayer.PrintMenu(_dispalyOptions);
                string option = Console.ReadLine();
                if (option == REGISTER_TO_GLOBAL_CHAT)
                {
                    RegisterToGlobalChat();
                }
                else if(option == REGISTER_TO_PRIVATE_CHAT)
                {
                    RegisterToPrivateChat();
                }
                else if (option == GET_ALL_CHATS)
                {
                    PrintAllChats();
                }
                else if(option == CREATE_GROUP_CHAT)
                {
                    RegisterToGroupChat();
                }
            }
        }

        private void RegisterToGlobalChat()
        {
            _client.CurrentChat = new GlobalChat(_client.BytesReceived, _client, _client.ConsoleDisplayer);
            _client.CurrentChat.WriteMessage(REGISTER_TO_GLOBAL_CHAT);
            _client.CurrentChat.ShowOptions();
            _client.CurrentChat.ReadFromServer();
            _client.CurrentChat.Run();
        }

        private void RegisterToPrivateChat()
        {
            _client.CurrentChat = new PrivateChat(_client.BytesReceived, _client, _client.ConsoleDisplayer);
            _client.CurrentChat.WriteMessage(REGISTER_TO_PRIVATE_CHAT);
            _client.CurrentChat.Run();
        }

        private void RegisterToGroupChat()
        {
            _client.CurrentChat = new GroupChat(_client.BytesReceived, _client, _client.ConsoleDisplayer);
            _client.CurrentChat.WriteMessage(CREATE_GROUP_CHAT);
            _client.CurrentChat.Run();
        }

        private void PrintAllChats()
        {
            _client.SendMessageToServer(GET_ALL_CHATS);
            string allChats = _client.GetMessageFromServer();
            if(!string.IsNullOrEmpty(allChats))
            {
                string[] allClientChats = allChats.Split(',');
                foreach (var chat in allClientChats)
                {
                    _consoleDisplayer.PrintValueToConsole(chat);
                }
            }
            else
            {
                _consoleDisplayer.PrintValueToConsole("You have no chats");
            }
        }
    }
}
