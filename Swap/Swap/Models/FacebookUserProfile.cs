namespace Swap.Models
{
    public class FacebookUserProfile
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public FBPicture Picture { get; set; }

        public class FBPicture
        {
            public Data Data { get; set; }
        }

        public class Data
        {
            public string Url { get; set; }
        }
    }
}
