using WEB_coursework_Site.Models;

namespace WEB_coursework_Site.DB.Entities
{
    public class PostWithDateModel
    {
        public List<PostModel>? PostModels { get; set; }

        public DateTimeOffset EldestDate { get; set; }
    }
}
