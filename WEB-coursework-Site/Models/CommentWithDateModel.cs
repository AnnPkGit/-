namespace WEB_coursework_Site.Models
{
    public class CommentWithDateModel
    {
        public List<CommentModel> Comments { get; set; }

        public DateTime EldestDate { get; set; }
    }
}
