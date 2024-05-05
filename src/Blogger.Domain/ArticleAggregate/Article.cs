﻿using Blogger.Domain.CommentAggregate;
using Blogger.Domain.Events.ArticleCreated;

namespace Blogger.Domain.ArticleAggregate;

public class Article : AggregateRootBase<ArticleId>
{

    public Article(ArticleId slug):base(slug)
    {  
        _tags = [];
        _commentIds = [];
    }

    private Article()  : this(null) { }


    private IList<CommentId> _commentIds = null!;
    public IReadOnlyCollection<CommentId> CommentIds => _commentIds.ToImmutableList();

    private IList<Tag> _tags = null!;
    public IReadOnlyCollection<Tag> Tags => _tags.ToImmutableList();

    public Author Author { get; private set; } = null!;

    public string Title { get; private set; } = null!;

    public string Body { get; private set; } = null!;

    public string Summary { get; private set; } = null!;

    public DateTime PublishedOnUtc { get; set; } = DateTime.MinValue;

    public ArticleStatus Status { get; private set; }

    public TimeSpan? ReadOn { get; private set; }

    public int GetReadOnInMinutes => Convert.ToInt32(ReadOn?.TotalMinutes);

    public static Article CreateDraft(string title, string body, string summary)
    {
        return new Article(ArticleId.CreateUniqueId(title))
        {
            Author = Author.CreateDefaultAuthor(),
            Body = body,
            Status = ArticleStatus.Draft,
            Summary = summary,
            Title = title,
        };
    }

    public static Article CreateArticle(string title, string body, string summary)
    {
        var article = new Article(ArticleId.CreateUniqueId(title))
        {
            Author = Author.CreateDefaultAuthor(),
            Body = body,
            Status = ArticleStatus.Published,
            Summary = summary,
            Title = title,
            ReadOn = GetReadOnTimeSpan(body),
            PublishedOnUtc = DateTime.UtcNow
        };

        article.AddEvent(new ArticleCreatedEvent() { Article = article  });

        return article;
    }

    public void AddTags(IReadOnlyList<Tag> tags)
    {
        foreach (var tag in tags)
        {
            _tags.Add(tag);
        }
    }

    private static TimeSpan GetReadOnTimeSpan(string body)
    {
        var readingTime = Math.Round(((double)body.Split(" ").Length / 200) * 60);
        return TimeSpan.FromSeconds(readingTime);
    }

    public void UpdateDraft(string title, string summary, string body)
    {
        Title = title;
        Body = body;
        Summary = summary;
    }

    public void UpdateTags(IReadOnlyList<Tag> tags)
    {
        if (_tags is not null)
            _tags.Clear();

        AddTags(tags);
    }

    public void Publish()
    {
        if (!_tags.Any())
        {
            throw new DraftTagsMissingException();
        }

        Status = ArticleStatus.Published;
        ReadOn = GetReadOnTimeSpan(Body);
        PublishedOnUtc = DateTime.UtcNow;
    }
    public void Remove()
    {
        Status = ArticleStatus.Deleted;
    }
}

public enum ArticleStatus
{
    Draft = 1,
    Published = 2,
    Deleted
}
