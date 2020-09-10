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
        private const string EXIT_PROGRAM = "exit";
        private const char API_COMMAND = '/';
        private Dictionary<string, string> _dispalyOptions;
        private Dictionary<ChatOptions, Action> _actionsToPerform;
        private ConsoleDisplayer _consoleDisplayer;
        private Client _client;
        public ClientManager(Dictionary<string, string> options)
        {
            _dispalyOptions = options;
            _consoleDisplayer = new ConsoleDisplayer();
            _client = new Client();
        }

        public void GenerateActionsDictionary()
        {
            _actionsToPerform = new Dictionary<ChatOptions, Action>();
            _actionsToPerform.Add(ChatOptions.Global, RegisterToGlobalChat);
            _actionsToPerform.Add(ChatOptions.Private, RegisterToPrivateChat);
            _actionsToPerform.Add(ChatOptions.Group, RegisterToGroupChat);
        }

        public void InitializeClient()
        {
            GenerateActionsDictionary();
            _client.CreateClient();
        }

        public void NavigateToChoice()
        {
            while (true)
            {
                _consoleDisplayer.PrintMenu(_dispalyOptions);
                string option = Console.ReadLine();
                int chatOption;
                if(int.TryParse(option, out chatOption))
                {

                    //if ((ChatOptions)chatOption == ChatOptions.SeeAll)
                    //{
                    //    _client.PrintAllChats(_consoleDisplayer);
                    //}
                    //else
                    //{
                    //    _actionsToPerform[(ChatOptions)chatOption].Invoke();
                    //}

                    if (option == REGISTER_TO_GLOBAL_CHAT)
                    {
                        RegisterToGlobalChat();
                    }
                    else if (option == REGISTER_TO_PRIVATE_CHAT)
                    {
                        RegisterToPrivateChat();
                    }
                    else if (option == GET_ALL_CHATS)
                    {
                        PrintAllChats();
                    }
                    else if (option == CREATE_GROUP_CHAT)
                    {
                        RegisterToGroupChat();
                    }
                }
                else if (option == EXIT_PROGRAM)
                {
                    _client.TcpClient.Close();
                    Environment.Exit(0);
                }
                else if (option[0] == API_COMMAND)
                {
                    JokesApi translateToKlingon = new JokesApi();
                    _consoleDisplayer.PrintValueToConsole(translateToKlingon.SendRequest());
                }
                else
                {
                    _consoleDisplayer.PrintValueToConsole("Please enter a valid option");
                }
            }
        }

        private void RegisterToGlobalChat()
        {
            _client.CurrentChat = new GlobalChat(_client.BytesReceived, _client, _client.ConsoleDisplayer);
            _client.CurrentChat.WriteMessage(((int)ChatOptions.Global).ToString());
            _client.CurrentChat.ShowOptions();
            _client.CurrentChat.Run();
        }

        private void RegisterToPrivateChat()
        {
            _client.CurrentChat = new PrivateChat(_client.BytesReceived, _client, _client.ConsoleDisplayer);
            _client.CurrentChat.WriteMessage(((int)ChatOptions.Private).ToString());
            _client.CurrentChat.Run();
        }

        private void RegisterToGroupChat()
        {
            _client.CurrentChat = new GroupChat(_client.BytesReceived, _client, _client.ConsoleDisplayer);
            _client.CurrentChat.WriteMessage(((int)ChatOptions.Group).ToString());
            _client.CurrentChat.Run();
        }

        public void PrintAllChats()
        {
            _client.SendMessageToServer(((int)ChatOptions.SeeAll).ToString());
            string allChats = _client.GetMessageFromServer();
            if (!string.IsNullOrEmpty(allChats))
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
