using Microsoft.AspNetCore.Mvc;
using WEB_coursework_Site.DB.Context;
using WEB_coursework_Site.Models;

namespace WEB_coursework_Site.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ISiteDbContextHelper _siteDbContextHelper;
        public UserController(ISiteDbContextHelper siteDbContextHelper)
        {
            _siteDbContextHelper = siteDbContextHelper ?? throw new NullReferenceException("ISiteDbContextHelper is null at UserController.cs");
        }

        [HttpPost]
        public async Task<string> PostAsync([FromBody] UserModel userModel)
        {
            var result = await _siteDbContextHelper.AddUserAsync(userModel);
            if (!result.IsSuccessful)
            {
                return result.Message;
            }
            return "OK";
        }
    }
}
