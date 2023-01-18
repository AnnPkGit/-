using Microsoft.AspNetCore.Http;
using WEB_coursework_Site.DB.Entities;
using WEB_coursework_Site.Helpers.Results;
using WEB_coursework_Site.Models;

namespace WEB_coursework_Site.DB.Validators
{
    public class EntityValidator : IEntityValidator
    {
        const int _maxCharsInPost = 500;

        public Result<User> CreateAndValidateUser(UserModel userModel)
        {
            var newUser = new User();
            try
            {
                newUser.RealName = String.IsNullOrEmpty(userModel.RealName) ? throw new Exception("invalid RealName") : userModel.RealName;
                newUser.Email = String.IsNullOrEmpty(userModel.Email) ? throw new Exception("invalid Email") : userModel.Email;
                newUser.Login = String.IsNullOrEmpty(userModel.Login) ? throw new Exception("invalid Login") : userModel.Login;
                newUser.Password = String.IsNullOrEmpty(userModel.Password) ? throw new Exception("invalid Password") : userModel.Password;
                newUser.SecretQuestionAnswear = String.IsNullOrEmpty(userModel.SecretQuestionAnswear) ? throw new Exception("SecretQuestionAnswear")
                    : userModel.SecretQuestionAnswear;
                newUser.Avatar = userModel.Avatar?.FileName;
            }
            catch (Exception ex)
            {
                return ResultCreator<User>.CreateFailedResult(ex.Message);
            }
            return ResultCreator<User>.CreateSuccessfulResult(newUser);
        }

        public Result<Post> CreateAndValidatePost(PostToAddModel postModel)
        {
            var newPost = new Post();
            try
            {
                newPost.Text = postModel.Text.Length > _maxCharsInPost 
                    ? throw new Exception($"Post can not contain text with more than {_maxCharsInPost} cars in it") : postModel.Text;
                newPost.Date = DateTimeOffset.UtcNow;
                newPost.Id = Guid.NewGuid();
            }
            catch (Exception ex)
            {
                return ResultCreator<Post>.CreateFailedResult(ex.Message);
            }
            return ResultCreator<Post>.CreateSuccessfulResult(newPost);
        }
    }
}
