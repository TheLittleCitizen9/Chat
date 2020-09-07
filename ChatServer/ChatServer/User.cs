using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ChatServer
{
    public class User
    {
        public TcpClient ClientSocket;
        public List<TcpClient> ClientsInChatWith;
        public int Id;

        public User(TcpClient tcpClient, int id)
        {
            ClientSocket = tcpClient;
            ClientsInChatWith = new List<TcpClient>();
            Id = id;
        }

        public void RemoveClient(TcpClient tcpClient)
        {
            if(ClientsInChatWith.Contains(tcpClient))
            {
                ClientsInChatWith.Remove(tcpClient);
            }
        }
    }
}
