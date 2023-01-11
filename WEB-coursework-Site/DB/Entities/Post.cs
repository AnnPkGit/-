namespace WEB_coursework_Site.DB.Entities
{
    public class Post
    {
        public Guid Id { get; set; }

        public string? Text { get; set; }

        public List<string>? Images { get; set; }

        public int Likes { get; set; }

        public int Comments { get; set; }
    }
}