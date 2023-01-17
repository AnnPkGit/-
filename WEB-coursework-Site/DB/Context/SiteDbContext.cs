using Microsoft.EntityFrameworkCore;
using WEB_coursework_Site.DB.Entities;

namespace WEB_coursework_Site.DB.Context
{
    public class SiteDbcontext : DbContext 
    {
        public DbSet<User> Users { get; set; }

        public DbSet<SecretQuestion> SecretQuestions { get; set; }

        public DbSet<Post> Posts { get; set; }

        public DbSet<Image> PostImages { get; set; }

        public DbSet<Token> Tokens { get; set; }

        public SiteDbcontext(DbContextOptions options)
            : base(options) { }
    }
}