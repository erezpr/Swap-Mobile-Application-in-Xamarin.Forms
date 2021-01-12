using System;
using System.Runtime.Serialization;

namespace Swap.Chat_Database
{
    [DataContract]
    public class InstantMessage
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public Guid ChatId { get; set; }
        [DataMember]
        public int UserId { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string Body { get; set; }

        public Chat Chat { get; set; }

        public DateTime Time { get; set; } = DateTime.Now;

        public InstantMessage() { }
    }
}