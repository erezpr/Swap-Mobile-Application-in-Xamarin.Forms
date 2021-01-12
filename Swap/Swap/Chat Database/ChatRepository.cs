using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Swap.Chat_Database
{
    public class ChatRepository : Repository<Chat>, IChatRepository
    {
        private ChatContext _chatContext => _context as ChatContext;

        private const int guidAreEqual = 0;

        public ChatRepository(ChatContext context) : base(context) { }

        public Chat Get(string guid)
        {
            return _chatContext.Chats.Include(c => c.UsersToGroup).Include(c => c.Messages)
                .Where(c => c.Id.ToString() == guid).FirstOrDefault();
        }
    }
}
