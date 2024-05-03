﻿using Blogger.Domain.Common.Exceptions;

namespace Blogger.Application.Articles.CreateArticle;

public sealed class ArticleAlreadyExistsException : BlogException
{
    private const string _messages = "Article with Title `{0}` already exists.";
    public ArticleAlreadyExistsException(string articleId)
        : base(string.Format(_messages, articleId))
    {
    }
}
