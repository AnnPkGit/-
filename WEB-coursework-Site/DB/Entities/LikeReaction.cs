namespace WEB_coursework_Site.DB.Entities
{
    public class LikeReaction
    {
        public Guid Id { get; set; }

        public Guid RelatedPostId { get; set; }

        public Guid RelatedUserId { get; set; }

        public int IsLiked { get; set; }
    }
}
