using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WEB_coursework_Site.DB.Context;

namespace WEB_coursework_Site.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReactionController : ControllerBase
    {
        private readonly ISiteDbContextHelper _siteDbContextHelper;
        public ReactionController(ISiteDbContextHelper siteDbContextHelper)
        {
            _siteDbContextHelper = siteDbContextHelper ?? throw new NullReferenceException("ISiteDbContextHelper is null at ReactionController .cs");
        }

        [HttpGet]
        [Route("post")]
        public async Task<string> AddLikeReactionPostAsync(string token, string postId, bool like)
        {
            var result = await _siteDbContextHelper.AddReactionAsync(token, Guid.Parse(postId), like);
            return JsonSerializer.Serialize(result);
        }

        [HttpGet]
        [Route("comment")]
        public async Task<string> AddReactionCommentAsync(string token, string postId, bool like)
        {
            var result = await _siteDbContextHelper.AddReactionCommentAsync(token, Guid.Parse(postId), like);
            return JsonSerializer.Serialize(result);
        }
    }
}
