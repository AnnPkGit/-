using WEB_coursework_Site.Helpers.Results;
using WEB_coursework_Site.Models;

namespace WEB_coursework_Site.DB.Context
{
    public interface ISiteDbContextHelper
    {
        Task<Result<string>> AddUserAsync(UserModel userModel);

        Task<string> AuthorizeUserAsync(UserModel userModel);

        Task<PostWithDateModel> GetPostsAsync(DateTimeOffset startTime, string token);

        Task<string> PostContentAsync(PostToAddModel postModel);

        Task<Result<PostModel>> GetPostById(Guid id, string token);

        Task<CommentWithDateModel> GetCommentsAsync(DateTimeOffset startTime, Guid postId, string token);

        Task<string> AddReactionAsync(string token, Guid postId, bool like);

        Task<string> AddReactionCommentAsync(string token, Guid commentId, bool like);
    }
}
