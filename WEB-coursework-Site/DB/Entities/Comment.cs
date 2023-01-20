namespace WEB_coursework_Site.DB.Entities
{
    public class Comment
    {
        public Guid Id { get; set; }

        public Guid AuthorId { get; set; }

        public Guid PostId { get; set; }

        public DateTimeOffset Date { get; set; }

        public int LikesCount { get; set; }

        public string Text { get; set; }
    }
}
