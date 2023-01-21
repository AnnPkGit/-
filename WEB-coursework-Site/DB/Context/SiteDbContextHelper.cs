using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
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

        public async Task<PostWithDateModel> GetPostsAsync(DateTimeOffset startTime, string token)
        {
            //с фронта приходит дата без offset и при конвертации в DateTimeOffset добавляется локальное смещение т.е. 2 часа
            //TODO: убрать необходимость добавления 2-х часов после конвертации в Utc т.к. это работает толь =ко для часового пояса +02:00
            var dbDateFormat = startTime != DateTimeOffset.MinValue ? startTime.UtcDateTime.AddHours(2) : startTime;
            return await Task.Run(() => GetPosts(dbDateFormat, token));
        }

        private PostWithDateModel GetPosts(DateTimeOffset startTime, string token)
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
            if (!String.IsNullOrEmpty(token))
            {
                var correspondingToken = _siteDbcontext.Tokens.Where(t => t.AccessToken.Equals(token)).FirstOrDefault();
                foreach (var postModel in postModels)
                {
                    var reaction = _siteDbcontext.LikeReactions.Where(l => l.RelatedUserId.Equals(correspondingToken.RelatedUserId)
                        && l.RelatedPostId.Equals(postModel.Id)).FirstOrDefault();
                    if (reaction != null && Convert.ToBoolean(reaction.IsLiked))
                        postModel.IsLiked = true;
                }
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

        public async Task<Result<PostModel>> GetPostById(Guid id, string token)
        {
            return await Task.Run(() => FindPost(id, token));
        }

        private async Task<Result<PostModel>> FindPost(Guid id, string token)
        {
            var post = await _siteDbcontext.Posts.FindAsync(id);
            if (post == null) 
            {
                return ResultCreator<PostModel>.CreateFailedResult("Not found");
            }
            
            var user = await _siteDbcontext.Users.FindAsync(post.AuthorId);
            if (user == null)
            {
                return ResultCreator<PostModel>.CreateFailedResult("No corresponding user");
            }

            var images = _siteDbcontext.PostImages.Where(i => i.RelatedPostId == post.Id).Select(i => i.Name).ToList();
            var isLiked = false;
            if (!String.IsNullOrEmpty(token))
            {
                var correspondingToken = _siteDbcontext.Tokens.Where(t => t.AccessToken.Equals(token)).FirstOrDefault();
                var reaction = _siteDbcontext.LikeReactions.Where(l => l.RelatedUserId.Equals(correspondingToken.RelatedUserId)
                        && l.RelatedPostId.Equals(id)).FirstOrDefault();
                if (reaction != null) isLiked = Convert.ToBoolean(reaction.IsLiked);
            }

            var postModel = new PostModel()
            {
                CommentsCount = post.CommentsCount,
                Date = post.Date,
                Id = post.Id,
                LikesCount = post.LikesCount,
                Text = post.Text,
                AuthorName = user.Login,
                Images = images.Any() ? images : null,
                AuthorAvatar = user.Avatar,
                IsLiked = isLiked
            };

            return ResultCreator<PostModel>.CreateSuccessfulResult(postModel);
        }

        public async Task<CommentWithDateModel> GetCommentsAsync(DateTimeOffset startTime, Guid postId, string token)
        {
            //с фронта приходит дата без offset и при конвертации в DateTimeOffset добавляется локальное смещение т.е. 2 часа
            //TODO: убрать необходимость добавления 2-х часов после конвертации в Utc т.к. это работает толь =ко для часового пояса +02:00
            var dbDateFormat = startTime != DateTimeOffset.MinValue ? startTime.UtcDateTime.AddHours(2) : startTime;
            return await Task.Run(() => GetComments( startTime, postId, token));
        }

        private CommentWithDateModel GetComments(DateTimeOffset startTime, Guid postId, string token)
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
            if (!String.IsNullOrEmpty(token))
            {
                var correspondingToken = _siteDbcontext.Tokens.Where(t => t.AccessToken.Equals(token)).FirstOrDefault();
                foreach (var comment in commentModels)
                {
                    var reaction = _siteDbcontext.LikeReactions.Where(l => l.RelatedUserId.Equals(correspondingToken.RelatedUserId)
                        && l.RelatedPostId.Equals(comment.Id)).FirstOrDefault();
                    if (reaction != null && Convert.ToBoolean(reaction.IsLiked))
                        comment.IsLiked = true;
                }
            }
            commentModels = commentModels.OrderByDescending(p => p.Date).ToList();

            commentsWithDateModel.Comments = commentModels;
            commentsWithDateModel.EldestDate = commentModels.Last().Date.DateTime;

            return commentsWithDateModel;
        }

        public async Task<string> AddReactionAsync(string token, Guid postId, bool like)
        {
            return await Task.Run(() => AddReactionToPostAsync(token, postId, like));
        }

        private async Task<string> AddReactionToPostAsync(string token, Guid postId, bool like)
        {
            var correspondingToken = _siteDbcontext.Tokens.Where(t => t.AccessToken.Equals(token)).FirstOrDefault();
            if (correspondingToken == null)
            {
                return "Access dinied";
            }
            var reaction = _siteDbcontext.LikeReactions.Where(r => r.RelatedUserId.Equals(correspondingToken.RelatedUserId) 
                && r.RelatedPostId.Equals(postId)).FirstOrDefault();
            var post = await _siteDbcontext.Posts.FindAsync(postId);
            if (reaction != null)
            {
                if((!like && Convert.ToBoolean(reaction.IsLiked)) || (like && !Convert.ToBoolean(reaction.IsLiked)))
                {
                    reaction.IsLiked = Convert.ToInt32(like);
                    post.LikesCount += Convert.ToBoolean(reaction.IsLiked) ? 1 : -1;
                }
            }
            else
            {
                reaction = new LikeReaction()
                {
                    Id = Guid.NewGuid(),
                    IsLiked = Convert.ToInt32(like),
                    RelatedPostId = postId,
                    RelatedUserId = correspondingToken.RelatedUserId
                };
                _siteDbcontext.LikeReactions.Add(reaction);

                if(Convert.ToBoolean(reaction.IsLiked))
                    post.LikesCount += 1;
            }
            await _siteDbcontext.SaveChangesAsync();

            return "Ok";
        }

        public async Task<string> AddReactionCommentAsync(string token, Guid commentId, bool like)
        {
            return await Task.Run(() => AddReactionToCommentAsync(token, commentId, like));
        }

        private async Task<string> AddReactionToCommentAsync(string token, Guid commentId, bool like)
        {
            var correspondingToken = _siteDbcontext.Tokens.Where(t => t.AccessToken.Equals(token)).FirstOrDefault();
            if (correspondingToken == null)
            {
                return "Access dinied";
            }
            var reaction = _siteDbcontext.LikeReactions.Where(r => r.RelatedUserId.Equals(correspondingToken.RelatedUserId)
                && r.RelatedPostId.Equals(commentId)).FirstOrDefault();
            var comment = await _siteDbcontext.Comments.FindAsync(commentId);
            if (reaction != null)
            {
                if ((!like && Convert.ToBoolean(reaction.IsLiked)) || (like && !Convert.ToBoolean(reaction.IsLiked)))
                {
                    reaction.IsLiked = Convert.ToInt32(like);
                    comment.LikesCount += Convert.ToBoolean(reaction.IsLiked) ? 1 : -1;
                }
            }
            else
            {
                reaction = new LikeReaction()
                {
                    Id = Guid.NewGuid(),
                    IsLiked = Convert.ToInt32(like),
                    RelatedPostId = commentId,
                    RelatedUserId = correspondingToken.RelatedUserId
                };
                _siteDbcontext.LikeReactions.Add(reaction);

                if (Convert.ToBoolean(reaction.IsLiked))
                    comment.LikesCount += 1;
            }
            await _siteDbcontext.SaveChangesAsync();

            return "Ok";
        }
    }
}