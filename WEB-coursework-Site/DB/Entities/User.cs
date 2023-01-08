namespace WEB_coursework_Site.DB.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        public string RealName { get; set; }

        public string Email { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public Guid SecretQuestionId { get; set; }

        public string SecretQuestionAnswear { get; set; }

        public string? Avatar { get; set; }

        public string PasswordSalt { get; set; }
    }
}
