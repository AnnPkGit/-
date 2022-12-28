namespace WEB_coursework_Site.Models
{
    public class UserModel
    {
        public string RealName { get; set; }

        public string Email { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public string SecretQuestion { get; set; }

        public IFormFile Avatar { get; set; }
    }
}
