using ChatClient.Chats;
using System;
using System.Collections.Generic;

namespace ChatClient
{
    public class ClientManager
    {
        private const string REGISTER_TO_GLOBAL_CHAT = "1";
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
            }
        }

        private void RegisterToGlobalChat()
        {
            _client.CurrentChat = new GlobalChat(_client.BytesReceived, _client.TcpClient, _client.ConsoleDisplayer);
            _client.CurrentChat.WriteMessage(REGISTER_TO_GLOBAL_CHAT);
            _client.CurrentChat.ShowOptions();
            _client.CurrentChat.ReadFromServer();
            _client.CurrentChat.Run();
        }
    }
}
