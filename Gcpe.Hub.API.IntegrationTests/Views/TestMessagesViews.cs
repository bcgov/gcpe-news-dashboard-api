using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Gcpe.Hub.API.IntegrationTests.Helpers;
using Gcpe.Hub.Data.Entity;
using Newtonsoft.Json;
using Xunit;

namespace Gcpe.Hub.API.IntegrationTests.Views
{
    public class MessagesViewsShould : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        public readonly HttpClient _client;
        public Message testMessage = MessagesTestData.CreateMessage("Test message title",
            "Test message description", true, false, 0);

        public MessagesViewsShould(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task List_EndpointReturnSuccessAndCorrectMessages()
        {
            var response = await _client.GetAsync("/api/messages");
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var deserializedBody = JsonConvert.DeserializeObject<Hub.API.ViewModels.MessageViewModel[]>(body);

            Assert.NotEmpty(deserializedBody);
            deserializedBody.Should().HaveCountGreaterOrEqualTo(MessagesTestData.seedMessageCount);
        }

        [Fact]
        public async Task Create_EndpointReturnSuccessAndCorrectMessage()
        {
            testMessage.Id = Guid.Empty;
            var stringContent = new StringContent(JsonConvert.SerializeObject(testMessage), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/messages",  stringContent);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var messageResult = JsonConvert.DeserializeObject<Hub.API.ViewModels.MessageViewModel>(body);

            messageResult.Title.Should().Equals(testMessage.Title);
            messageResult.Description.Should().Equals(testMessage.Description);
        }

        [Fact]
        public async Task Create_EndpointRequiresTitle()
        {
            testMessage.Id = Guid.Empty;
            var brokenTestMessage = testMessage;
            brokenTestMessage.Title = null;
            var stringContent = new StringContent(JsonConvert.SerializeObject(brokenTestMessage), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/messages", stringContent);
            response.StatusCode.Should().Equals("400");
        }

        [Fact]
        public async Task Create_EndpointDoesntRequireDescription()
        {
            testMessage.Id = Guid.Empty;
            var noDescMessage = testMessage;
            noDescMessage.Description = null;
            var stringContent = new StringContent(JsonConvert.SerializeObject(noDescMessage), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/messages", stringContent);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var messageResult = JsonConvert.DeserializeObject<Hub.API.ViewModels.MessageViewModel>(body);

            messageResult.Title.Should().Equals(noDescMessage.Title);
            messageResult.Description.Should().Equals(null);
        }

        [Fact]
        public async Task Get_EndpointReturnSuccessAndCorrectMessage()
        {
            testMessage.Id = Guid.Empty;
            var stringContent = new StringContent(JsonConvert.SerializeObject(testMessage), Encoding.UTF8, "application/json");
            var createResponse = await _client.PostAsync("/api/messages", stringContent);
            var createBody = await createResponse.Content.ReadAsStringAsync();
            var createdMessage = JsonConvert.DeserializeObject<Hub.API.ViewModels.MessageViewModel>(createBody);
            var id = createdMessage.Id;

            var response = await _client.GetAsync($"/api/Messages/{id}");
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var messageResult = JsonConvert.DeserializeObject<Hub.API.ViewModels.MessageViewModel>(body);

            messageResult.Title.Should().Equals(testMessage.Title);
            messageResult.Description.Should().Equals(testMessage.Description);
            messageResult.Id.Should().Equals(id);
        }

        [Fact]
        public async Task Get_EndpointNotFound()
        {
            var response = await _client.GetAsync($"/api/messages/{Guid.NewGuid()}");

            response.StatusCode.Equals("404");
        }

        [Fact]
        public async Task Put_EndpointReturnSuccessAndCorrectMessage()
        {
            testMessage.Id = Guid.Empty;
            var stringContent = new StringContent(JsonConvert.SerializeObject(testMessage), Encoding.UTF8, "application/json");
            var createResponse = await _client.PostAsync("/api/messages", stringContent);
            var createBody = await createResponse.Content.ReadAsStringAsync();
            var createdMessage = JsonConvert.DeserializeObject<Hub.API.ViewModels.MessageViewModel>(createBody);
            var id = createdMessage.Id;

            var newTestMessage = createdMessage;
            newTestMessage.Title = "New title";
            newTestMessage.Description = "New description";
            newTestMessage.SortOrder = 10;

            var content = new StringContent(JsonConvert.SerializeObject(newTestMessage), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"/api/messages/{id}", content);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var messageResult = JsonConvert.DeserializeObject<Hub.API.ViewModels.MessageViewModel>(body);

            messageResult.Title.Should().Equals(newTestMessage.Title);
            messageResult.Description.Should().Equals(newTestMessage.Description);
            messageResult.SortOrder.Should().Equals(newTestMessage.SortOrder);
            messageResult.Id.Should().Equals(id);
        }

        [Fact]
        public async Task Put_EndpointReturnSuccessWithDefaultsAndCorrectMessage()
        {
            testMessage.Id = Guid.Empty;
            var stringContent = new StringContent(JsonConvert.SerializeObject(testMessage), Encoding.UTF8, "application/json");
            var createResponse = await _client.PostAsync("/api/messages", stringContent);
            var createBody = await createResponse.Content.ReadAsStringAsync();
            var createdMessage = JsonConvert.DeserializeObject<Hub.API.ViewModels.MessageViewModel>(createBody);
            var id = createdMessage.Id;

            var title = "new title";
            var content = new StringContent(JsonConvert.SerializeObject(new { Title = title }), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"/api/messages/{id}", content);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var messageResult = JsonConvert.DeserializeObject<Hub.API.ViewModels.MessageViewModel>(body);

            messageResult.Title.Should().Equals(title);
            messageResult.Description.Should().Equals(null);
            messageResult.SortOrder.Should().Equals(0);
            messageResult.IsPublished.Should().BeFalse();
            messageResult.IsHighlighted.Should().BeFalse();
            messageResult.Id.Should().Equals(id);
        }

        [Fact]
        public async Task Put_EndpointShouldRequireTitle()
        {
            testMessage.Id = Guid.Empty;
            var stringContent = new StringContent(JsonConvert.SerializeObject(testMessage), Encoding.UTF8, "application/json");
            var createResponse = await _client.PostAsync("/api/messages", stringContent);
            var createBody = await createResponse.Content.ReadAsStringAsync();
            var createdMessage = JsonConvert.DeserializeObject<Hub.API.ViewModels.MessageViewModel>(createBody);
            var id = createdMessage.Id;
            
            var content = new StringContent(JsonConvert.SerializeObject(new { }), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"/api/messages/{id}", content);
            response.StatusCode.Should().Equals("400");
        }
    }
}

