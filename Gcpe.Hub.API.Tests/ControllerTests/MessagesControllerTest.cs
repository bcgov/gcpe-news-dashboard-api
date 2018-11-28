using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using Gcpe.Hub.API.Controllers;
using Gcpe.Hub.API.Data;
using Gcpe.Hub.API.ViewModels;
using Gcpe.Hub.Data.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Gcpe.Hub.API.Tests.ControllerTests
{
    public class MessagesControllerTests : IDisposable
    {
        private Mock<ILogger<MessagesController>> logger;
        private HubDbContext context;
        private IMapper mapper;
        private DbContextOptions<HubDbContext> options;

        public MessagesControllerTests()
        {
            this.options = new DbContextOptionsBuilder<HubDbContext>()
                      .UseInMemoryDatabase(Guid.NewGuid().ToString())
                      .Options;
            this.context = new HubDbContext(this.options);

            this.logger = new Mock<ILogger<MessagesController>>();

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            this.mapper = mockMapper.CreateMapper();
        }
        public void Dispose()
        {
            this.context.Dispose();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(3)]
        public void GetAll_ShouldReturnSuccess(int messageCount)
        {
            for (var i = 0; i < messageCount; i++)
            {
                context.Message.Add(TestData.TestMessage(i.ToString()));
            }
            context.SaveChanges();
            var controller = new MessagesController(context, logger.Object, mapper);

            var result = controller.GetAll() as ObjectResult;

            result.Should().BeOfType<OkObjectResult>();
            result.Should().NotBeNull();
            var models = result.Value as ICollection<Message>;
            models.Should().NotBeNull();
            models.Count().Should().Equals(messageCount);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetAll_ShouldDefaultIsPublishedParameterTrue(bool isPublished)
        {
            var publishedCount = 3;
            var unpublishedCount = 2;
            for (var i = 0; i < publishedCount; i++)
            {
                context.Message.Add(TestData.TestMessage($"published-{i.ToString()}"));
            }
            for (var i = 0; i < unpublishedCount; i++)
            {
                var testMessage = TestData.TestMessage($"unpublished-{i.ToString()}");
                testMessage.IsPublished = false;
                context.Message.Add(testMessage);
            }
            context.SaveChanges();
            var controller = new MessagesController(context, logger.Object, mapper);

            var result = controller.GetAll(isPublished) as ObjectResult;

            result.Should().BeOfType<OkObjectResult>();
            var models = result.Value as ICollection<Message>;
            models.Count().Should().Equals(isPublished ? publishedCount : unpublishedCount);
        }

        [Fact]
        public void GetAll_ShouldReturnBadRequest()
        {
            var mockContext = new Mock<HubDbContext>(this.options);
            mockContext.Setup(m => m.Message).Throws(new Exception());
            var controller = new MessagesController(mockContext.Object, logger.Object, mapper);

            var result = controller.GetAll() as ObjectResult;

            result.StatusCode.Should().Be(400);
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void Post_ShouldReturnSuccess()
        {
            var controller = new MessagesController(context, logger.Object, mapper);

            var result = controller.Post(mapper.Map<Message, MessageViewModel>(TestData.TestMessage("1"))) as ObjectResult;

            result.StatusCode.Should().Equals(201);
            result.Should().BeOfType<CreatedAtRouteResult>();
            var model = result.Value as MessageViewModel;
            model.Title.Should().Equals("2018MESSAGE-1");
        }

        [Fact]
        public void Post_ShouldReturnBadRequest()
        {
            var controller = new MessagesController(context, logger.Object, mapper);
            controller.ModelState.AddModelError("error", "some validation error");
            var testMessage = TestData.TestMessage("1");

            var result = controller.Post(message: null) as ObjectResult;

            result.Should().BeOfType<BadRequestObjectResult>();
            result.StatusCode.Should().Be(400);
        }

        [Fact]
        public void Get_ShouldReturnSuccess()
        {
            var controller = new MessagesController(context, logger.Object, mapper);
            var testMessage = TestData.TestMessage("1");
            context.Message.Add(testMessage);
            context.SaveChanges();

            var result = controller.Get(testMessage.Id) as ObjectResult;

            result.Should().BeOfType<OkObjectResult>();
            result.StatusCode.Should().Be(200);
            var model = result.Value as MessageViewModel;
            model.Title.Should().Equals("2018MESSAGE-1");
        }

        [Fact]
        public void Get_ShouldReturnFail()
        {
            var options = new DbContextOptionsBuilder<HubDbContext>()
                      .UseInMemoryDatabase(Guid.NewGuid().ToString())
                      .Options;
            var mockContext = new Mock<HubDbContext>(options);
            mockContext.Setup(m => m.Message).Throws(new Exception());
            var controller = new MessagesController(mockContext.Object, logger.Object, mapper);
            var testMessage = TestData.TestMessage("1");
            context.Message.Add(testMessage);
            context.SaveChanges();

            var result = controller.Get(testMessage.Id) as ObjectResult;

            result.Should().BeOfType<BadRequestObjectResult>();
            result.StatusCode.Should().Be(400);
        }

        [Fact]
        public void Get_ShouldReturnNotFound()
        {
            var controller = new MessagesController(context, logger.Object, mapper);

            var result = controller.Get(Guid.NewGuid()) as ObjectResult;

            result.Should().BeOfType<NotFoundObjectResult>();
            result.StatusCode.Should().Be(404);
        }
        
        [Fact]
        public void Put_ShouldReturnSuccess()
        {
            var testMessage = TestData.TestMessage("1");

            context.Message.Add(testMessage);
            context.SaveChanges();
            var testMessageVM = mapper.Map<Message, MessageViewModel>(testMessage);
            testMessageVM.Title = "New Title!";


            var controller = new MessagesController(context, logger.Object, mapper);
            var result = controller.Put(testMessage.Id, testMessageVM) as ObjectResult;

            result.Should().BeOfType<OkObjectResult>();
            result.StatusCode.Should().Be(200);
            var model = result.Value as MessageViewModel;
            model.Title.Should().Equals("New Title!");
            var dbMessage = context.Message.Find(testMessage.Id);
            dbMessage.Title.Should().Equals("New Title!");
        }

        [Fact]
        public void Put_ShouldReturnBadRequest()
        {
            var controller = new MessagesController(context, logger.Object, mapper);
            var testMessage = TestData.TestMessage("1");
            context.Message.Add(testMessage);
            context.SaveChanges();

            var result = controller.Put(testMessage.Id, message: null) as ObjectResult ;

            result.Should().BeOfType<BadRequestObjectResult>();
            result.StatusCode.Should().Be(400);
        }

        [Fact]
        public void Put_ShouldReturnNotFound()
        {
            var controller = new MessagesController(context, logger.Object, mapper);
            var testMessage = TestData.TestMessage("1");

            var result = controller.Put(testMessage.Id, message: null) as ObjectResult;

            result.Should().BeOfType<NotFoundObjectResult>();
            result.StatusCode.Should().Be(404);
        }
    }
}
