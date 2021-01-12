using System;
using System.Collections.Generic;
using System.Linq;

namespace Swap.Chat_Database
{
    public class InstantMessageRepository : Repository<InstantMessage>, IInstantMessageRepository
    {
        private ChatContext _instantMessageContext => _context as ChatContext;

        public InstantMessageRepository(ChatContext context) : base(context) { }

        public IEnumerable<InstantMessage> GetLastMessages(string chatId, int wantedMessageCount)
        {
            IEnumerable<InstantMessage> messagesOfChat = GetAll(m => m.ChatId.ToString() == chatId);
            int howManyMessagesChatHave = messagesOfChat.Count();
            wantedMessageCount = wantedMessageCount > howManyMessagesChatHave ? howManyMessagesChatHave : wantedMessageCount;
            return messagesOfChat.Skip(howManyMessagesChatHave - wantedMessageCount);
        }

        public IEnumerable<InstantMessage> GetAll(Func<InstantMessage, bool> predicate) => GetAll().Where(m => predicate(m));

        public InstantMessage GetLast()
        {
            return GetAll().LastOrDefault();
        }

        public InstantMessage GetLastMessageOfChat(string guid)
        {
            return GetAll(im => im.ChatId.ToString() == guid).LastOrDefault();
        }
    }
}