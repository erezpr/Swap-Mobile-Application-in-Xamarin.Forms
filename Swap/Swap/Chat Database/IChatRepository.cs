namespace Swap.Chat_Database
{
    public interface IChatRepository : IRepository<Chat>
    {
        Chat Get(string guid);
    }
}