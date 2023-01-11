namespace WEB_coursework_Site.DB.Entities
{
    public class Post
    {
        public Guid Id { get; set; }

        public string? Text { get; set; }

        public int LikesCount { get; set; }

        public int CommentsCount { get; set; }
    }
}