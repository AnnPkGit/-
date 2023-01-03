using WEB_coursework_Site.DB.Entities;
using WEB_coursework_Site.Helpers.Results;
using WEB_coursework_Site.Models;

namespace WEB_coursework_Site.DB.Validators
{
    public class EntityValidator : IEntityValidator
    {
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
    }
}
