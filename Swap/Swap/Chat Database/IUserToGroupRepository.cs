using System;
using System.Collections.Generic;

namespace Swap.Chat_Database
{
    public interface IUserToGroupRepository : IRepository<UserToGroup>
    {
        bool TryGetChatId(int fromId, int toId, out string groupName);

        IEnumerable<UserToGroup> GetAll(Func<UserToGroup, bool> func);

        UserToGroup Get(Func<UserToGroup, bool> predicate);
    }
}