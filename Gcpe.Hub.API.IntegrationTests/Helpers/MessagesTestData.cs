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

        public static Message CreateMessage(string title,
            string description, Boolean isPublished, Boolean isHighlighted,
            int sortOrder)
        {
            var message = new Message
            {
                Id = Guid.NewGuid(),
                Title = title,
                Description = description,
                IsPublished = isPublished,
                IsHighlighted = isHighlighted,
                SortOrder = sortOrder,
                Timestamp = DateTime.Now
            };

            return message;
        }

        public static void InitializeDbForTests(HubDbContext db)
        {
            for(var i = 0; i < seedMessageCount; i++)
            {
                var message = CreateMessage($"Title - {i}", $"Description - {i}", true, false, i);
                db.Message.Add(message);
            }
            db.SaveChanges();
        }
    }
}
