namespace WEB_coursework_Site.Models
{
    public class PostModel
    {
        public Guid Id { get; set; }

        public string? Text { get; set; }

        public int LikesCount { get; set; }

        public int CommentsCount { get; set; }

        public DateTimeOffset Date { get; set; }

        public string AuthorName { get; set; }

        public string? AuthorAvatar { get; set; }

        public List<string>? Images { get; set; }

        public bool IsLiked { get; set; }
    }
}
