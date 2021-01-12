namespace Swap.Models
{
    public class Trade
    {
        public int Id { get; set; }
        public int OfferedToId { get; set; }
        public int ItemId { get; set; }
        public int OfferedById { get; set; }
        public string ItemsToTrade { get; set; }
        public int? Status { get; set; }
    }
}