using BasicChatContents;
using System;

namespace ChatClient.Chats
{
    public class GlobalChat : BasicChat
    {
        public GlobalChat(Byte[] bytes, Client tcpClient, ConsoleDisplayer consoleDisplayer) 
            :base(bytes, tcpClient, consoleDisplayer)
        {

        }
        public override void Run()
        {
            base.ReadFromServer();
            WriteMessage();
        }
    }
}
