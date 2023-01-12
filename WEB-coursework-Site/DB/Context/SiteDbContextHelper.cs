using Microsoft.EntityFrameworkCore;
using System.Linq;
using WEB_coursework_Site.DB.Entities;
using WEB_coursework_Site.DB.Validators;
using WEB_coursework_Site.Helpers.Hasher;
using WEB_coursework_Site.Helpers.Results;
using WEB_coursework_Site.Models;

namespace WEB_coursework_Site.DB.Context
{
    public class SiteDbContextHelper : ISiteDbContextHelper
    {
        private readonly SiteDbcontext _siteDbcontext;
        private readonly IEntityValidator _entityValidator;
        private readonly IHasher _hasher;
        private const int _postPerPage = 20;

        public SiteDbContextHelper(SiteDbcontext siteDbcontext, IEntityValidator entityValidator, IHasher hasher)
        {
            _siteDbcontext = siteDbcontext ?? throw new NullReferenceException("SiteDbcontext is null at SiteDbContextHelper.cs");
            _entityValidator = entityValidator ?? throw new NullReferenceException("IEntityValidator is null at SiteDbContextHelper.cs");
            _hasher = hasher ?? throw new NullReferenceException("IHasher is null at SiteDbContextHelper.cs");
        }

        public async Task<Result<string>> AddUserAsync(UserModel userModel)
        {
            var creationResult = _entityValidator.CreateAndValidateUser(userModel);
            if(!creationResult.IsSuccessful)
            {
                return ResultCreator<string>.CreateFailedResult($"Failed to add user. Reason: {creationResult.Message}");
            }
            if (_siteDbcontext.Users.Where(u => u.Login.Equals(userModel.Login)).FirstOrDefault() != null)
            {
                return ResultCreator<string>.CreateFailedResult($"Failed to add user. Reason: login is already in use");
            }

            var secretQuestion = _siteDbcontext.SecretQuestions.Where(q => q.Question.Equals(userModel.SecretQuestion)).FirstOrDefault();
            if(secretQuestion == null)
            {
                return ResultCreator<string>.CreateFailedResult($"Failed to add user. Reason: no such secret question exists");
            }

            var userToAdd = creationResult.Value;
            userToAdd.SecretQuestionId = secretQuestion.Id;
            userToAdd.PasswordSalt = _hasher.Hash(DateTime.Now.ToString());
            userToAdd.Password = _hasher.Hash(userToAdd.Password + userToAdd.PasswordSalt);

            var result = AddUserAndSaveChangesAsync(userToAdd);
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

        public async Task<PostWithDateModel> GetPostsAsync(DateTimeOffset startTime)
        {
            return await Task.Run(() => GetPosts(startTime));
        }

        private PostWithDateModel GetPosts(DateTimeOffset startTime)
        {
            var posts = new List<Post>();
            if (startTime == DateTimeOffset.MinValue)
            {
                posts = _siteDbcontext.Posts.Select(p => p).OrderByDescending(p => p.Date).Take(_postPerPage).ToList();
            }
            else
            {
                posts = _siteDbcontext.Posts.Where(p => DateTimeOffset.Compare(p.Date, startTime) < 0)
                    .OrderByDescending(p => p.Date).Take(_postPerPage).ToList();
            }

            var postsWithUsers = from user in _siteDbcontext.Users.Select(u => u).ToList()
                                 join post in posts on user.Id equals post.AuthorId
                                 select new { Post = post, User = user};

            var relatedImages = from image in _siteDbcontext.PostImages.Select(i => i).ToList()
                            group image by image.RelatedPostId into g
                            select new { Key = g.Key, Images = g.ToList() };

            var postModels = new List<PostModel>(20);
            foreach (var postWuser in postsWithUsers)
            {
                postModels.Add(new PostModel()
                {
                    AuthorName = postWuser.User.Login,
                    CommentsCount = postWuser.Post.CommentsCount,
                    Date = postWuser.Post.Date,
                    Id = postWuser.Post.Id,
                    LikesCount = postWuser.Post.LikesCount,
                    Text = postWuser.Post.Text,
                    Images = relatedImages.Where( k => k.Key == postWuser.Post.Id)
                    .Select( i => i.Images.Select( b => b.Name).ToList()).FirstOrDefault(),
                    AuthorAvatar = postWuser.User.Avatar
                });
            }

            var postWithDateModels = new PostWithDateModel()
            {
                PostModels = postModels,
                EldestDate = postModels.Any() ? postModels.Last().Date : DateTimeOffset.MinValue
            };

            return postWithDateModels;
        }
    }
}