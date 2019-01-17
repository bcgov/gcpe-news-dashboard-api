<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;
using Gcpe.Hub.Data.Entity;

namespace Gcpe.Hub.API.Tests.ControllerTests
{
    public static class TestData
    {
        public static NewsRelease CreateDbPost(string key = "2018PREM1234-123456", string pageTitle = "Test title")
        {
            var release = new NewsRelease
            {
                Id = Guid.NewGuid(),
                Key = key,
                ReleaseType = ReleaseType.Story,
                Timestamp = DateTime.Now,
                ReleaseDateTime = DateTime.Now,
                PublishDateTime = DateTime.Now,
                IsPublished = true,
                IsActive = true,
                IsCommitted = true,
                Ministry = new Ministry { Id = Guid.NewGuid(), Key = "environment", DisplayName = "Environment" },
                NewsReleaseLanguage = new List<NewsReleaseLanguage>{new NewsReleaseLanguage { Summary = "summary", LanguageId = 4105 } }
            };

            release.NewsReleaseDocument = new List<NewsReleaseDocument>
            {
                new NewsReleaseDocument
                {
                    Id = Guid.NewGuid(),
                    Release = release,
                    NewsReleaseDocumentLanguage = new List<NewsReleaseDocumentLanguage>
                    {
                        new NewsReleaseDocumentLanguage { PageTitle = pageTitle }
                    }
                }
            };
            return release;
        }

        public static ICollection<NewsReleaseLog> CreateDbPostLogs(NewsRelease release, string author = "Jane Doe")
        {
            return new List<NewsReleaseLog>
            {
                new NewsReleaseLog
                {
                    Id = 1,
                    Description = "Created by " + author,
                    DateTime = DateTime.Now.AddDays(-1),
                    Release = release
                },
                new NewsReleaseLog {
                    Id = 2,
                    Description = "Edited by " + author,
                    DateTime = DateTime.Now,
                    Release = release
                }
            };
        }

        public static Activity CreateDbActivity(string title, string details, int id)
        {
            return new Activity
            {
                Title = title,
                Details = details,
                IsActive = true,
                IsConfirmed = true,
                Id = id,
                StartDateTime = DateTime.Today.AddDays(3),
                ActivityCategories = new List<ActivityCategories>
                {
                    new ActivityCategories
                    {
                         ActivityId = id,
                         Category = new Category
                         {
                            Name = "Release Only (No Event)"
                         }
                    }
                }
            };
        }

        public static Message CreateDbMessage(string title, string description,
            int sortOrder, Guid? id = null, bool isPublished = false, bool isHighlighted = false)
        {
            return new Message
            {
                Id = id ?? Guid.Empty,
=======
﻿using System;
using System.Collections.Generic;
using Gcpe.Hub.Data.Entity;

namespace Gcpe.Hub.API.Tests.ControllerTests
{
    public static class TestData
    {
        public static List<NewsRelease> TestNewsReleases
        {
            get
            {
                var releases = new List<NewsRelease>();

                for (var i = 0; i < 10; i++)
                {
                    var releaseId = Guid.NewGuid();

                    var release = new NewsRelease
                    {
                        Id = releaseId,
                        Key = $"2018PREM{i}-{i}00000",
                        Year = 2018,
                        Timestamp = DateTime.Now,
                        ReleaseDateTime = DateTime.Now,
                        PublishDateTime = DateTime.Now,
                        IsPublished = true,
                        IsActive = true,
                        IsCommitted = true,
                        Keywords = "lorem, ipsum, dolor",
                        NewsReleaseLog = new List<NewsReleaseLog>
                    {
                        new NewsReleaseLog
                        {
                            Id = 1,
                            Description = "Created by Jane Doe",
                            DateTime = DateTime.Now,
                            ReleaseId = releaseId
                        },
                        new NewsReleaseLog {
                            Id = 2,
                            Description = "Edited by John Doe",
                            DateTime = DateTime.Now,
                            ReleaseId = releaseId
                        }
                    }
                    };

                    releases.Add(release);
                }

                return releases;
            }
        }

        public static NewsRelease TestNewsRelease
        {
            get
            {
                var releaseId = Guid.NewGuid();

                return new NewsRelease
                {
                    Id = releaseId,
                    Key = "2018PREM1234-123456",
                    Year = 2018,
                    Timestamp = DateTime.Now,
                    ReleaseDateTime = DateTime.Now,
                    PublishDateTime = DateTime.Now,
                    IsPublished = true,
                    IsActive = true,
                    IsCommitted = true,
                    Keywords = "lorem, ipsum, dolor",
                    NewsReleaseLog = new List<NewsReleaseLog>
                    {
                        new NewsReleaseLog
                        {
                            Id = 1,
                            Description = "Created by Jane Doe",
                            DateTime = DateTime.Now,
                            ReleaseId = releaseId
                        },
                        new NewsReleaseLog {
                            Id = 2,
                            Description = "Edited by John Doe",
                            DateTime = DateTime.Now,
                            ReleaseId = releaseId
                        }
                    }
                };
            }
        }
        public static Message CreateMessage(string title, string description,
            int sortOrder, bool isPublished = false, bool isHighlighted = false)
        {
            var message = new Message
            {
                Id = Guid.Empty,
>>>>>>> master
                Title = title,
                Description = description,
                SortOrder = sortOrder,
                IsPublished = isPublished,
                IsHighlighted = isHighlighted,
<<<<<<< HEAD
                Timestamp = DateTime.Now,
                IsActive = true
            };
        }

        public static SocialMediaPost CreateDbSocialMediaPost(string url, Guid? id = null, bool isActive = true, int sortOrder = 0)
        {
            return new SocialMediaPost
            {
                Id = id ?? Guid.Empty,
                Url = url,
                SortOrder = sortOrder,
                Timestamp = DateTime.Now,
                IsActive = isActive
            };
        }
    }
}
=======
                Timestamp = DateTime.Now
            };

            return message;
        }

    }
}
>>>>>>> master
