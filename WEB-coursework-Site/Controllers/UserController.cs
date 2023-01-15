using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
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
                return JsonSerializer.Serialize(result.Message);
            }
            return JsonSerializer.Serialize("OK");
        }

        [HttpPost]
        [Route("authorization")]
        public async Task<string> AuthorizeAsync([FromBody] UserModel userModel)
        {
            var result = await _siteDbContextHelper.AuthorizeUserAsync(userModel);
            return JsonSerializer.Serialize(result);
        }
    }
}
