namespace WEB_coursework_Site.DB.Entities
{
    public class Image
    {
        public Guid Id { get; set; }

        public Guid RelatedPostId { get; set; }

        public string Name { get; set; }
    }
}
