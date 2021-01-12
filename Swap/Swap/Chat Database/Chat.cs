using System;
using System.Collections.Generic;

namespace Swap.Chat_Database
{
    public class Chat
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        public ICollection<InstantMessage> Messages { get; set; }

        public ICollection<UserToGroup> UsersToGroup { get; set; }

        public Chat() { } 

        public Chat(ICollection<UserToGroup> users) => UsersToGroup = users;
    }
}