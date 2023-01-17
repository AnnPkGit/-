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
        private const int _postPerPage = 10;

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
            //с фронта приходит дата без offset и при конвертации в DateTimeOffset добавляется локальное смещение т.е. 2 часа
            //TODO: убрать необходимость добавления 2-х часов после конвертации в Utc т.к. это работает толь =ко для часового пояса +02:00
            var dbDateFormat = startTime != DateTimeOffset.MinValue ? startTime.UtcDateTime.AddHours(2) : startTime;
            return await Task.Run(() => GetPosts(dbDateFormat));
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
                EldestDate = postModels.Any() ? postModels.Last().Date.DateTime : DateTimeOffset.MinValue.DateTime
            };

            return postWithDateModels;
        }

        public async Task<string> AuthorizeUserAsync(UserModel userModel)
        {
            return await Task.Run(() => FindUser(userModel));
        }

        public async Task<string> FindUser(UserModel userModel)
        {
            var user = _siteDbcontext.Users.Where(u => u.Login.Equals(userModel.Login)).FirstOrDefault();
            if (user != null && _hasher.Hash(userModel.Password + user.PasswordSalt).Equals(user.Password))
            {
                var token = _siteDbcontext.Tokens.Where(t => t.RelatedUserId.Equals(user.Id)).FirstOrDefault()?.AccessToken;
                if(String.IsNullOrEmpty(token))
                {
                    token = $"{Guid.NewGuid()}_token";
                    await _siteDbcontext.Tokens.AddAsync(new Token() { AccessToken = token, RelatedUserId = user.Id, Id = Guid.NewGuid() });
                    await _siteDbcontext.SaveChangesAsync();
                }
                return token;
            }
            return "User does not exist";
        }
    }
}