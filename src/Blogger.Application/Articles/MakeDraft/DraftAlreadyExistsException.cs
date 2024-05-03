﻿using Blogger.Domain.Common.Exceptions;

namespace Blogger.Application.Articles.MakeDraft;

public sealed class DraftAlreadyExistsException : BlogException
{
    private const string _messages = "Draft with Title `{0}` already exists.";
    public DraftAlreadyExistsException(string articleId)
        : base(string.Format(_messages, articleId))
    {
    }
}
