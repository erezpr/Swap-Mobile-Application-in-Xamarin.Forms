using System;
using System.Collections.Generic;
using System.Linq;

namespace Swap.Chat_Database
{
    public class UserToGroupRepository : Repository<UserToGroup>, IUserToGroupRepository
    {
        private ChatContext _userToGroupContext => _context as ChatContext;

        public UserToGroupRepository(ChatContext context) : base(context) { }

        public bool TryGetChatId(int fromId, int toId, out string chatId)
        {
            foreach (UserToGroup userToGroupOfSender in GetAll(u => u.UserId == fromId))
            {
                foreach (UserToGroup userToGroupOfReceiver in GetAll(u => u.UserId == toId))
                {
                    if (userToGroupOfSender.Guid.ToString() == userToGroupOfReceiver.Guid.ToString())
                    {
                        chatId = userToGroupOfSender.Guid.ToString();
                        return true;
                    }

                }
            }

            chatId = null;
            return false;
        }

        public IEnumerable<UserToGroup> GetAll(Func<UserToGroup, bool> func) => GetAll().Where(u => func(u));

        public UserToGroup Get(Func<UserToGroup, bool> predicate)
        {
            return GetAll(predicate).FirstOrDefault();
        }
    }
}