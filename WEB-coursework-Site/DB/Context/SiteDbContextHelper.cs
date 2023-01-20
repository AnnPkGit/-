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
            if (!creationResult.IsSuccessful)
            {
                return ResultCreator<string>.CreateFailedResult($"Failed to add user. Reason: {creationResult.Message}");
            }
            if (_siteDbcontext.Users.Where(u => u.Login.Equals(userModel.Login)).FirstOrDefault() != null)
            {
                return ResultCreator<string>.CreateFailedResult($"Failed to add user. Reason: login is already in use");
            }

            var secretQuestion = _siteDbcontext.SecretQuestions.Where(q => q.Question.Equals(userModel.SecretQuestion)).FirstOrDefault();
            if (secretQuestion == null)
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
            catch (Exception ex)
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
                posts = (from t in _siteDbcontext.Posts
                         orderby t.Date descending
                         select t).Take(_postPerPage).ToList();
            }
            else
            {
                posts = (from t in _siteDbcontext.Posts
                         where t.Date < startTime
                         orderby t.Date descending
                         select t).Take(_postPerPage).ToList();
            }

            var postWithDateModels = new PostWithDateModel();
            if (!posts.Any())
                return postWithDateModels;

            var postsWithUsers = from user in _siteDbcontext.Users.Select(u => u).ToList()
                                 join post in posts on user.Id equals post.AuthorId
                                 select new { Post = post, User = user };

            var relatedImages = from image in _siteDbcontext.PostImages.Select(i => i).ToList()
                                group image by image.RelatedPostId into g
                                select new { Key = g.Key, Images = g.ToList() };

            var postModels = new List<PostModel>(_postPerPage);
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
            postModels = postModels.OrderByDescending(p => p.Date).ToList();

            postWithDateModels.PostModels = postModels;
            postWithDateModels.EldestDate = postModels.Last().Date.DateTime;

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

        public async Task<string> PostContentAsync(PostToAddModel postModel)
        {
            var creationResult = _entityValidator.CreateAndValidatePost(postModel);
            if (!creationResult.IsSuccessful)
            {
                return $"Failed to add post. Reason: {creationResult.Message}";
            }
            var postToAdd = creationResult.Value;

            var relatedUserResult = _siteDbcontext.Users.Where(u => u.Login.Equals(postModel.Login)).FirstOrDefault();
            if (relatedUserResult == null)
            {
                return "Failed to add post. Reason: no corresponding user";
            }

            var token = _siteDbcontext.Tokens.Where(t => t.RelatedUserId.Equals(relatedUserResult.Id) 
                && t.AccessToken.Equals(postModel.AccessToken)).FirstOrDefault();
            if(token == null)
            {
                return "Access denied";
            }


            postToAdd.AuthorId = relatedUserResult.Id;
            var result = await AddPostAndSaveChangesAsync(postToAdd);
            return "Ok";
        }

        private async Task<Result<string>> AddPostAndSaveChangesAsync(Post post)
        {
            try
            {
                await _siteDbcontext.Posts.AddAsync(post);
                await _siteDbcontext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ResultCreator<string>.CreateFailedResult(ex.Message);
            }
            return ResultCreator<string>.CreateSuccessfulResult("Ok");
        }

        public async Task<CommentWithDateModel> GetCommentsAsync(DateTimeOffset startTime, Guid postId)
        {
            //с фронта приходит дата без offset и при конвертации в DateTimeOffset добавляется локальное смещение т.е. 2 часа
            //TODO: убрать необходимость добавления 2-х часов после конвертации в Utc т.к. это работает толь =ко для часового пояса +02:00
            var dbDateFormat = startTime != DateTimeOffset.MinValue ? startTime.UtcDateTime.AddHours(2) : startTime;
            return await Task.Run(() => GetComments( startTime, postId));
        }

        private CommentWithDateModel GetComments(DateTimeOffset startTime, Guid postId)
        {
            var comments = new List<Comment>();
            if (startTime == DateTimeOffset.MinValue)
            {
                comments = (from t in _siteDbcontext.Comments
                         where t.PostId.Equals(postId)
                         orderby t.Date descending
                         select t).Take(_postPerPage).ToList();
            }
            else
            {
                comments = (from t in _siteDbcontext.Comments
                         where t.Date < startTime && t.PostId.Equals(postId)
                         orderby t.Date descending
                         select t).Take(_postPerPage).ToList();
            }

            var commentsWithDateModel = new CommentWithDateModel();
            if (!comments.Any())
                return commentsWithDateModel;

            var commentsWithUsers = from user in _siteDbcontext.Users.Select(u => u).ToList()
                                 join comment in comments on user.Id equals comment.AuthorId
                                 select new { Comment = comment, User = user };

            var relatedImages = from image in _siteDbcontext.PostImages.Select(i => i).ToList()
                                group image by image.RelatedPostId into g
                                select new { Key = g.Key, Images = g.ToList() };

            var commentModels = new List<CommentModel>(_postPerPage);
            foreach (var commentWuser in commentsWithUsers)
            {
                commentModels.Add(new CommentModel()
                {
                    AuthorName = commentWuser.User.Login,
                    Date = commentWuser.Comment.Date,
                    Id = commentWuser.Comment.Id,
                    LikesCount = commentWuser.Comment.LikesCount,
                    Text = commentWuser.Comment.Text,
                    Images = relatedImages.Where(k => k.Key == commentWuser.Comment.Id)
                    .Select(i => i.Images.Select(b => b.Name).ToList()).FirstOrDefault(),
                    AuthorAvatar = commentWuser.User.Avatar
                });
            }
            commentModels = commentModels.OrderByDescending(p => p.Date).ToList();

            commentsWithDateModel.Comments = commentModels;
            commentsWithDateModel.EldestDate = commentModels.Last().Date.DateTime;

            return commentsWithDateModel;
        }
    }
}