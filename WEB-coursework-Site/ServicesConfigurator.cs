using WEB_coursework_Site.DB.Context;
using WEB_coursework_Site.DB.Validators;
using WEB_coursework_Site.Helpers.Hasher;

namespace WEB_coursework_Site
{
    static public class ServicesConfigurator
    {
        static public void ConfigureLocalServices(IServiceCollection services)
        {
            services.AddScoped<ISiteDbContextHelper, SiteDbContextHelper>();
            services.AddScoped<IEntityValidator, EntityValidator>();
            services.AddScoped<IHasher, Hasher>();
        }
    }
}