﻿using BasicChatContents;
using System;
using System.Collections.Generic;

namespace ChatClient.Chats
{
    public class GroupChat : BasicChat
    {
        private ChatUtils _chatUtils;
        public GroupChat(Byte[] bytes, Client tcpClient, ConsoleDisplayer consoleDisplayer)
            : base(bytes, tcpClient, consoleDisplayer)
        {
            _chatUtils = new ChatUtils();
        }

        public override void ShowOptions()
        {
            _options.Add("add admin", "To add a user as admin");
            _options.Add("remove", "To remove a user from the group");
            _options.Add("leave", "To leave the group");
            base.ShowOptions();
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
                _chatUtils.PrintClientsConnected(allClients);
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
            WriteMessage(chatName);
        }

        private void SendServerClientIds(string ids)
        {
            WriteMessage(ids);
        }

        private bool ValidateIds(string[] ids, string[] allClients)
        {
            List< bool> result = new List<bool>();
            foreach (var id in ids)
            {
                result.Add(_chatUtils.ValidateId(id, allClients));
            }
            return !result.Contains(false);
        }
    }
}
