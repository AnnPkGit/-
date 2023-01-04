using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WEB_coursework_Site.DB.Entities;
using WEB_coursework_Site.DB.Validators;
using WEB_coursework_Site.Helpers.Results;
using WEB_coursework_Site.Models;

namespace WEB_coursework_Site.DB.Context
{
    public class SiteDbContextHelper : ISiteDbContextHelper
    {
        private readonly SiteDbcontext _siteDbcontext;
        private readonly IEntityValidator _entityValidator;
        public SiteDbContextHelper(SiteDbcontext siteDbcontext, IEntityValidator entityValidator)
        {
            _siteDbcontext = siteDbcontext ?? throw new NullReferenceException("SiteDbcontext is null at SiteDbContextHelper.cs");
            _entityValidator = entityValidator ?? throw new NullReferenceException("IEntityValidator is null at SiteDbContextHelper.cs"); ;
        }

        public async Task<Result<string>> AddUserAsync(UserModel userModel)
        {
            var creationResult = _entityValidator.CreateAndValidateUser(userModel);
            if(!creationResult.IsSuccessful)
            {
                return ResultCreator<string>.CreateFailedResult($"Failed to add user. Reason: {creationResult.Message}");
            }

            var secretQuestion = _siteDbcontext.SecretQuestions.Where(q => q.Question.Equals(userModel.SecretQuestion)).FirstOrDefault();
            if(secretQuestion == null)
            {
                return ResultCreator<string>.CreateFailedResult($"Failed to add user. Reason: no such secret question exists");
            }

            var userToAdd = creationResult.Value;
            userToAdd.SecretQuestionId = secretQuestion.Id.ToString();

            var result =  AddUserAndSaveChangesAsync(userToAdd);
            return await result;
        }

        private async Task<Result<string>> AddUserAndSaveChangesAsync(User user)
        {
            try
            {
                await _siteDbcontext.Users.AddAsync(user);
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