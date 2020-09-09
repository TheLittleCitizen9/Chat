using System.Net.Sockets;
using System.Text;

namespace ChatServer.Handlers
{
    public class ClientHandler
    {
        public void SendClientAllHisChats(User user)
        {
            string allChatsOfClient = string.Empty;
            foreach (var chat in user.AllChats)
            {
                allChatsOfClient += $"{chat.Name.Replace("\0", string.Empty)}-{chat.ChatOption},";
            }
            if (!string.IsNullOrEmpty(allChatsOfClient))
            {
                SendClientMessage(allChatsOfClient, user);
            }
            else
            {
                SendClientMessage("No chats", user);
            }
        }

        public void SendClientMessage(string message, User user)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            NetworkStream nwStream = user.ClientSocket.GetStream();
            nwStream.Write(data);
        }
    }
}
