using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ChatServer.ChatManagers
{
    public abstract class BasicChat
    {
        public List<User> UsersInChat { get; set; }
        public GeneralChatFunctions ChatFunctions { get; set; }

        public BasicChat()
        {
            UsersInChat = new List<User>();
        }
        public void SendMessageToClients(string dataToSend)
        {
            byte[] data = Encoding.ASCII.GetBytes(dataToSend);
            foreach (var client in UsersInChat)
            {
                try
                {
                    NetworkStream nwStream = client.ClientSocket.GetStream();
                    nwStream.Write(data);
                }
                catch (Exception)
                {
                    ChatFunctions.ConsoleDisplayer.PrintValueToConsole($"Client {client.Id} disconnected");
                }
            }
        }

        public void RemoveClientFromReceivingMessages(User user, Guid id)
        {
            user.AddInactiveChatId(id);
            ChatFunctions.RemoveClient(user, UsersInChat, id);
            SendMessageToClients($"Client {user.Id} left chat");
        }
    }
}
