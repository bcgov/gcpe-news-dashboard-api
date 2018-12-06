using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Gcpe.Hub.API.ViewModels;
using Newtonsoft.Json;
using Xunit;

namespace Gcpe.Hub.API.IntegrationTests.Views
{
    public class TestSocialMediaViews: IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        public readonly HttpClient _client;
        public SocialMediaPostViewModel testPost = TestData.CreateSocialMediaPost(url: "http://facebook.com/post/123");

        public TestSocialMediaViews(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task List_EndpointShouldReturnSuccessAndCorrectPost()
        {
            for (var i = 0; i < 5; i++)
            {
                var newPost = TestData.CreateSocialMediaPost("http://facebook.com/post/123");
                var stringContent = new StringContent(JsonConvert.SerializeObject(newPost), Encoding.UTF8, "application/json");

                var createResponse = await _client.PostAsync("/api/socialmedia", stringContent);
                createResponse.EnsureSuccessStatusCode();
            }

            var response = await _client.GetAsync("/api/socialmedia");
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var deserializedBody = JsonConvert.DeserializeObject<SocialMediaPostViewModel[]>(body);

            Assert.NotEmpty(deserializedBody);
            deserializedBody.Should().HaveCountGreaterOrEqualTo(5);
        }

        [Fact]
        public async Task Create_EndpointShouldReturnSuccessAndCorrectPost()
        {
            var stringContent = new StringContent(JsonConvert.SerializeObject(testPost), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/socialmedia", stringContent);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var messageResult = JsonConvert.DeserializeObject<SocialMediaPostViewModel>(body);

            messageResult.Url.Should().Be(testPost.Url);
        }

        [Fact]
        public async Task Create_EndpointShouldRequireUrl()
        {
            var brokenTestPost = testPost;
            brokenTestPost.Url = null;
            var stringContent = new StringContent(JsonConvert.SerializeObject(brokenTestPost), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/socialmedia", stringContent);
            response.IsSuccessStatusCode.Should().BeFalse();
        }

        [Fact]
        public async Task Get_EndpointShouldReturnSuccessAndCorrectPost()
        {
            var stringContent = new StringContent(JsonConvert.SerializeObject(testPost), Encoding.UTF8, "application/json");
            var createResponse = await _client.PostAsync("/api/socialmedia", stringContent);
            var createBody = await createResponse.Content.ReadAsStringAsync();
            var createdPost = JsonConvert.DeserializeObject<SocialMediaPostViewModel>(createBody);
            var id = createdPost.Id;

            var response = await _client.GetAsync($"/api/socialmedia/{id}");
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var postResult = JsonConvert.DeserializeObject<SocialMediaPostViewModel>(body);

            postResult.Url.Should().Be(postResult.Url);
            postResult.Id.Should().Be(id);
        }

        [Fact]
        public async Task Get_EndpointShouldReturnNotFound()
        {
            var response = await _client.GetAsync($"/api/socialmedia/{Guid.NewGuid()}");

            response.IsSuccessStatusCode.Should().BeFalse();
        }

        [Fact]
        public async Task Put_EndpointShouldReturnSuccessAndCorrectMessage()
        {
            var stringContent = new StringContent(JsonConvert.SerializeObject(testPost), Encoding.UTF8, "application/json");
            var createResponse = await _client.PostAsync("/api/socialmedia", stringContent);
            var createBody = await createResponse.Content.ReadAsStringAsync();
            var createdPost = JsonConvert.DeserializeObject<SocialMediaPostViewModel>(createBody);
            var id = createdPost.Id;

            var newPost = TestData.CreateSocialMediaPost("http://twitter.com/post/123");

            var content = new StringContent(JsonConvert.SerializeObject(newPost), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"/api/socialmedia/{id}", content);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var postResult = JsonConvert.DeserializeObject<SocialMediaPostViewModel>(body);

            postResult.Url.Should().Be(postResult.Url);
            postResult.Id.Should().Be(id);
        }

        [Fact]
        public async Task Put_EndpointShouldShouldRequireUrl()
        {
            var stringContent = new StringContent(JsonConvert.SerializeObject(testPost), Encoding.UTF8, "application/json");
            var createResponse = await _client.PostAsync("/api/socialmedia", stringContent);
            var createBody = await createResponse.Content.ReadAsStringAsync();
            var createdPost = JsonConvert.DeserializeObject<SocialMediaPostViewModel>(createBody);
            var id = createdPost.Id;

            var content = new StringContent(JsonConvert.SerializeObject(new { }), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"/api/socialmedia/{id}", content);
            response.IsSuccessStatusCode.Should().BeFalse();
        }
    }
}
