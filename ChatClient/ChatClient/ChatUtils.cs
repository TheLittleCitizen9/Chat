using BasicChatContents;

namespace ChatClient
{
    public class ChatUtils
    {
        private ConsoleDisplayer _consoleDisplayer;
        public ChatUtils()
        {
            _consoleDisplayer = new ConsoleDisplayer();
        }
        public void PrintClientsConnected(string[] clients)
        {
            foreach (var client in clients)
            {
                _consoleDisplayer.PrintValueToConsole(client);
            }
        }

        public bool ValidateId(string id, string[] allClients)
        {
            foreach (var client in allClients)
            {
                if (client.Contains(id))
                    return true;
            }
            return false;
        }
    }
}
