using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using AutoMapper;
using System;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gcpe.Hub.API.Controllers;
using Gcpe.Hub.API.ViewModels;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Gcpe.Hub.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Gcpe.Hub.API.Data;

namespace Gcpe.Hub.API.Tests.ControllerTests
{
    public class SocialMediaControllerTests
    {
        private Mock<ILogger<SocialMediaController>> logger;
        private HubDbContext context;
        private IMapper mapper;

        public SocialMediaControllerTests()
        {

            this.context = GetContext();
            this.logger = new Mock<ILogger<SocialMediaController>>();
            this.mapper = CreateMapper();
        }

        private HubDbContext GetContext()
        {
            var options = new DbContextOptionsBuilder<HubDbContext>()
                      .UseInMemoryDatabase(Guid.NewGuid().ToString())
                      .Options;
            var context = new HubDbContext(options);
            return context;
        }

        private IMapper CreateMapper()
        {
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            var mapper = mockMapper.CreateMapper();
            return mapper;
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(3)]
        public void GetAll_ShouldReturnSuccess(int postCount)
        {
            for (var i = 0; i < postCount; i++)
            {
                var post = TestData.CreateSocialMediaPost("http://facebook.com/post/123");
                post.Id = Guid.NewGuid();
                context.SocialMediaPost.Add(post);
            }
            context.SaveChanges();
            var controller = new SocialMediaController(context, logger.Object, mapper);

            var result = controller.GetAll();
            var okResult = result as ObjectResult;

            okResult.Should().BeOfType<OkObjectResult>();
            okResult.Should().NotBeNull();

            var models = okResult.Value as ICollection<SocialMediaPost>;
            models.Should().NotBeNull();
            models.Count().Should().Be(postCount);
        }

        [Fact]
        public void GetAll_ShouldReturnBadRequest()
        {
            var options = new DbContextOptionsBuilder<HubDbContext>()
                      .UseInMemoryDatabase(Guid.NewGuid().ToString())
                      .Options;
            var mockContext = new Mock<HubDbContext>(options);
            mockContext.Setup(m => m.SocialMediaPost).Throws(new Exception());
            var controller = new SocialMediaController(mockContext.Object, logger.Object, mapper);

            var result = controller.GetAll() as ObjectResult;

            result.Should().BeOfType<BadRequestObjectResult>();
            result.StatusCode.Should().Be(400);
        }

        [Fact]
        public void Post_ShouldReturnSuccess()
        {

            var controller = new SocialMediaController(context, logger.Object, mapper);

            var result = controller.Post(postVM: mapper.Map<SocialMediaPost, SocialMediaPostViewModel>(TestData.CreateSocialMediaPost("http://facebook.com/post/123")));
            var createdResult = result as ObjectResult;

            createdResult.Should().BeOfType<CreatedAtRouteResult>();
            createdResult.StatusCode.Should().Be(201);

            var model = createdResult.Value as SocialMediaPostViewModel;
            model.Url.Should().Be(TestData.CreateSocialMediaPost("http://facebook.com/post/123").Url);
        }


        [Fact]
        public void Post_ShouldReturnBadRequest()
        {
            var controller = new SocialMediaController(context, logger.Object, mapper);
            controller.ModelState.AddModelError("error", "some validation error");
            var testSocialMediaPost = TestData.CreateSocialMediaPost("http://facebook.com/post/123");

            var result = controller.Post(postVM: null) as ObjectResult;

            result.Should().BeOfType<BadRequestObjectResult>();
            result.StatusCode.Should().Be(400);
        }

        [Fact]
        public void Get_ShouldReturnSuccess()
        {
            var controller = new SocialMediaController(context, logger.Object, mapper);
            var testSocialMediaPost = TestData.CreateSocialMediaPost("http://facebook.com/post/123");
            context.SocialMediaPost.Add(testSocialMediaPost);
            context.SaveChanges();

            var result = controller.Get(testSocialMediaPost.Id) as ObjectResult;

            result.Should().BeOfType<OkObjectResult>();
            result.StatusCode.Should().Be(200);

            var model = result.Value as SocialMediaPostViewModel;
            model.Url.Should().Be(testSocialMediaPost.Url);
        }

        [Fact]
        public void Get_ShouldReturnFail()
        {
            var options = new DbContextOptionsBuilder<HubDbContext>()
                      .UseInMemoryDatabase(Guid.NewGuid().ToString())
                      .Options;
            var mockContext = new Mock<HubDbContext>(options);
            mockContext.Setup(m => m.SocialMediaPost).Throws(new Exception());
            var controller = new SocialMediaController(mockContext.Object, logger.Object, mapper);

            var testSocialMediaPost = TestData.CreateSocialMediaPost("http://facebook.com/post/123");
            context.SocialMediaPost.Add(testSocialMediaPost);
            context.SaveChanges();
       
            var result = controller.Get(testSocialMediaPost.Id) as ObjectResult;

            result.Should().BeOfType<BadRequestObjectResult>();
            result.StatusCode.Should().Be(400);
        }

        [Fact]
        public void Get_ShouldReturnNotFound()
        {
            var controller = new SocialMediaController(context, logger.Object, mapper);

            var result = controller.Get(Guid.NewGuid()) as ObjectResult;

            result.Should().BeOfType<NotFoundObjectResult>();
            result.StatusCode.Should().Be(404);
        }

        [Fact]
        public void Put_ShouldReturnSuccess()
        {
            var testPost = TestData.CreateSocialMediaPost("http://facebook.com/post/123");

            context.SocialMediaPost.Add(testPost);
            context.SaveChanges();
            var socialMediaPostVM = mapper.Map<SocialMediaPost, SocialMediaPostViewModel>(testPost);
            socialMediaPostVM.Url = "http://twitter.com/post/123";


            var controller = new SocialMediaController(context, logger.Object, mapper);
            var result = controller.Put(testPost.Id, socialMediaPostVM) as ObjectResult;

            result.Should().BeOfType<OkObjectResult>();
            result.StatusCode.Should().Be(200);
            var model = result.Value as SocialMediaPostViewModel;
            model.Url.Should().Be("http://twitter.com/post/123");
            var dbMessage = context.SocialMediaPost.Find(testPost.Id);
            dbMessage.Url.Should().Be("http://twitter.com/post/123");
        }

        [Fact]
        public void Put_ShouldReturnBadRequest()
        {
            var controller = new SocialMediaController(context, logger.Object, mapper);
            var testSocialMediaPost = TestData.CreateSocialMediaPost("http://facebook.com/post/123");
            context.SocialMediaPost.Add(testSocialMediaPost);
            context.SaveChanges();

            var result = controller.Put(testSocialMediaPost.Id, postVM: null) as ObjectResult;

            result.Should().BeOfType<BadRequestObjectResult>();
            result.StatusCode.Should().Be(400);
        }

        [Fact]
        public void Put_ShouldReturnNotFound()
        {
            var controller = new SocialMediaController(context, logger.Object, mapper);
            var testSocialMediaPost = TestData.CreateSocialMediaPost("http://facebook.com/post/123");

            var result = controller.Put(testSocialMediaPost.Id, postVM: null) as ObjectResult;

            result.Should().BeOfType<NotFoundObjectResult>();
            result.StatusCode.Should().Be(404);
        }
    }
}
