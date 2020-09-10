using System.Collections.Generic;

namespace ChatClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, string> options = new Dictionary<string, string>();
            options.Add("0", "See all Chats");
            options.Add("1", "Enter Global Chat");
            options.Add("2", "Create Private Chat");
            options.Add("3", "Create Group Chat");
            options.Add("exit", "Exit Program");
            options.Add("/{something}", "To See Something Funny");
            ClientManager clientManager = new ClientManager(options);
            clientManager.InitializeClient();
            clientManager.NavigateToChoice();
        }
    }
}
