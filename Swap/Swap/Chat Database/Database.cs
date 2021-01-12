namespace Swap.Chat_Database
{
    public class Database
    {
        public ChatRepository ChatsTable { get; private set; }
        public UserToGroupRepository UserToGroupTable { get; private set; }
        public InstantMessageRepository InstantMessageTable { get; private set; }

        public Database()
        {
            ChatContext chatContext = new ChatContext();
            chatContext.Database.EnsureCreated();

            ChatsTable = new ChatRepository(chatContext);
            UserToGroupTable = new UserToGroupRepository(chatContext);
            InstantMessageTable = new InstantMessageRepository(chatContext);

        }
    }
}