using Microsoft.EntityFrameworkCore;
using WEB_coursework_Site.DB.Entities;

namespace WEB_coursework_Site.DB.Context
{
    public class SiteDbcontext : DbContext 
    {
        public DbSet<User> Users { get; set; }

        public SiteDbcontext(DbContextOptions options)
            : base(options) { }
    }
}