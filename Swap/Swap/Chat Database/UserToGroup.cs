using System;

namespace Swap.Chat_Database
{
    public class UserToGroup
    {
        public int Id { get; set; }

        public Guid Guid { get; set; }

        public Chat Chat { get; set; }

        public int UserId { get; set; }

        public string Username { get; set; }

        public UserToGroup() { }

        public UserToGroup(int i_userId, string i_userName)
        {
            UserId = i_userId;
            Username = i_userName;
        }
    }
}