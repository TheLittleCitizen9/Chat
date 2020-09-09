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
        //public List<User> OtherUsersInChat { get; set; }
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
                catch (Exception e)
                {
                    ChatFunctions.ConsoleDisplayer.PrintValueToConsole(e.Message);
                }
            }
        }
    }
}
