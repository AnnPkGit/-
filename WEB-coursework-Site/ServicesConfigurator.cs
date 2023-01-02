using Microsoft.Extensions.DependencyInjection;
using WEB_coursework_Site.DB.Context;

namespace WEB_coursework_Site
{
    static public class ServicesConfigurator
    {
        static public void ConfigureLocalServices(IServiceCollection services)
        {
            services.AddScoped<ISiteDbContextHelper, SiteDbContextHelper>();
        }
    }
}