using Microsoft.AspNetCore.Mvc;
using WEB_coursework_Site.DB.Context;
using WEB_coursework_Site.Models;

namespace WEB_coursework_Site.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ISiteDbContextHelper _siteDbContextHelper;

        public CommentController(ISiteDbContextHelper siteDbContextHelper)
        {
            _siteDbContextHelper = siteDbContextHelper ?? throw new NullReferenceException("ISiteDbContextHelper is null at PostController.cs");
        }

        [HttpGet]
        public async Task<CommentWithDateModel> GetAsync(DateTimeOffset startTime, Guid postId, string? token)
        {
            var result = _siteDbContextHelper.GetCommentsAsync(startTime, postId, token);
            try
            {
                return await result;
            }
            catch
            {
                return new CommentWithDateModel();
            }
        }
    }
}
