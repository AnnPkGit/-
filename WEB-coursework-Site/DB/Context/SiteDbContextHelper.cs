using WEB_coursework_Site.DB.Entities;
using WEB_coursework_Site.Helpers.Results;
using WEB_coursework_Site.Models;

namespace WEB_coursework_Site.DB.Context
{
    public class SiteDbContextHelper : ISiteDbContextHelper
    {
        private readonly SiteDbcontext _siteDbcontext;
        public SiteDbContextHelper(SiteDbcontext siteDbcontext)
        {
            _siteDbcontext = siteDbcontext ?? throw new NullReferenceException("SiteDbcontext is null at SiteDbContextHelper.cs");
        }

        public async Task<Result<string>> AddUserAsync(UserModel userModel)
        {
            var newUser = new User() 
            { 
                Id = Guid.NewGuid(),
                RealName = userModel.RealName,
                Email = userModel.Email,
                Login = userModel.Login,
                Password = userModel.Password,
                //SecretQuestionId 
                //SecretQuestionAnswear
                Avatar = userModel.Avatar?.FileName
            };

            try
            {
                await _siteDbcontext.Users.AddAsync(newUser);
                await _siteDbcontext.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                return ResultCreator<string>.CreateFailedResult(ex.Message);
            }
            return ResultCreator<string>.CreateSuccessfulResult("Ok");
        }
    }
}