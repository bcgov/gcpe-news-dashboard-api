using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gcpe.Hub.Data.Entity;

namespace Gcpe.Hub.API.IntegrationTests.Helpers
{
    public class MessagesTestData
    {
        public static int seedMessageCount = 10;

        public static Message CreateMessage(string title, string description,
            int sortOrder, bool isPublished = false, bool isHighlighted = false)
        {
            var message = new Message
            {
                Id = Guid.Empty,
                Title = title,
                Description = description,
                SortOrder = sortOrder,
                IsPublished = isPublished,
                IsHighlighted = isHighlighted,
                Timestamp = DateTime.Now
            };

            return message;
        }

        public static void InitializeDbForTests(HubDbContext db)
        {
            for(var i = 0; i < seedMessageCount; i++)
            {
                var message = CreateMessage($"Title - {i}", $"Description - {i}", i, true, false);
                message.Id = Guid.NewGuid();
                db.Message.Add(message);
            }
            db.SaveChanges();
        }
    }
}
