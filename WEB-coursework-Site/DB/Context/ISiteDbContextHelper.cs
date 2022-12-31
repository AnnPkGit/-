using WEB_coursework_Site.Helpers.Results;
using WEB_coursework_Site.Models;

namespace WEB_coursework_Site.DB.Context
{
    public interface ISiteDbContextHelper
    {
        Result<string> AddUser(UserModel userModel);
    }
}
