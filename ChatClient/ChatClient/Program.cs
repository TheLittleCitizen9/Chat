using System.Collections.Generic;

namespace ChatClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, string> options = new Dictionary<string, string>();
            options.Add("1", "Global chat");
            options.Add("2", "Private chat");
            ClientManager clientManager = new ClientManager(options);
            clientManager.InitializeClient();
            clientManager.NavigateToChoice();
        }
    }
}
