using System.Collections.Generic;
using System.Linq;

namespace ChatServer.Handlers
{
    public class GeneralHandler
    {
        public bool CheckIfAListContainsAnother(List<User> users, List<User> otherUsers, List<User> usersInChat)
        {
            if(!users.Equals(otherUsers))
            {
                var allUsersInChat = otherUsers.Select(u => u).ToList();
                allUsersInChat.AddRange(usersInChat);
                var result = users.Intersect(allUsersInChat).ToList();
                if(result.Count != users.Count)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
