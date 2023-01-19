namespace WEB_coursework_Site.Models
{
    public class PostWithDateModel
    {
        public List<PostModel>? PostModels { get; set; }

        public DateTime EldestDate { get; set; }
    }
}
