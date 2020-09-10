using ChatServer.Chats;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace ChatServer
{
    public class User
    {
        public TcpClient ClientSocket;
        public List<TcpClient> ClientsInChatWith;
        public int Id;
        public List<Guid> ActiveChatIds;
        public List<Guid> InactiveChatIds;
        public List<Chat> AllChats;

        public User(TcpClient tcpClient, int id)
        {
            ClientSocket = tcpClient;
            ClientsInChatWith = new List<TcpClient>();
            Id = id;
            ActiveChatIds = new List<Guid>();
            InactiveChatIds = new List<Guid>();
            AllChats = new List<Chat>();
        }

        public void RemoveClient(TcpClient tcpClient)
        {
            if(ClientsInChatWith.Contains(tcpClient))
            {
                ClientsInChatWith.Remove(tcpClient);
            }
        }

        public void AddActiveChatId(Guid id)
        {
            if(InactiveChatIds.Contains(id))
            {
                ActiveChatIds.Add(id);
                InactiveChatIds.Remove(id);
            }
            else
            {
                ActiveChatIds.Add(id);
            }
        }

        public void AddNumbChatId(Guid id)
        {
            InactiveChatIds.Add(id);
            ActiveChatIds.Remove(id);
        }

        public void AddChat(Chat chat)
        {
            AllChats.Add(chat);
        }
    }
}
