using WEB_coursework_Site.DB.Entities;
using WEB_coursework_Site.Helpers.Results;
using WEB_coursework_Site.Models;

namespace WEB_coursework_Site.DB.Validators
{
    public interface IEntityValidator
    {
        Result<User> CreateAndValidateUser(UserModel userModel);

        Result<Post> CreateAndValidatePost(PostToAddModel postModel);
    }
}
