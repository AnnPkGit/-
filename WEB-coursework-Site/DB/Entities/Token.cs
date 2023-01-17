namespace WEB_coursework_Site.DB.Entities
{
    public class Token
    {
        public Guid Id { get; set;  }

        public string AccessToken { get; set; }

        public Guid RelatedUserId { get; set; }
    }
}
