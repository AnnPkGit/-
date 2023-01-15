﻿using WEB_coursework_Site.DB.Entities;
using WEB_coursework_Site.Helpers.Results;
using WEB_coursework_Site.Models;

namespace WEB_coursework_Site.DB.Context
{
    public interface ISiteDbContextHelper
    {
        Task<Result<string>> AddUserAsync(UserModel userModel);

        Task<PostWithDateModel> GetPostsAsync(DateTimeOffset startTime);
    }
}