using System;
using System.Collections.Generic;

namespace Swap.Chat_Database
{
    public interface IInstantMessageRepository : IRepository<InstantMessage>
    {
        InstantMessage GetLast();

        IEnumerable<InstantMessage> GetAll(Func<InstantMessage, bool> predicate);

        IEnumerable<InstantMessage> GetLastMessages(string chatId, int wantedNumberOfMessages);

        InstantMessage GetLastMessageOfChat(string guid);
    }
}