using System;
using System.Collections.Generic;

namespace BasicChatContents
{
    public class ConsoleDisplayer
    {
        public void PrintValueToConsole(object obj)
        {
            Console.WriteLine(obj);
        }
        public void PrintMenu(Dictionary<string, string> options)
        {
            Console.WriteLine("Enter one option from the menu");
            foreach (KeyValuePair<string, string> option in options)
            {
                Console.WriteLine($"{option.Key} - {option.Value}");
            }
        }
    }
}
