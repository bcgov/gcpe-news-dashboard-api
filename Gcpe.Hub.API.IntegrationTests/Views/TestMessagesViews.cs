using System;
<<<<<<< HEAD
using System.Linq;
=======
>>>>>>> master
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
<<<<<<< HEAD
=======
using Gcpe.Hub.API.IntegrationTests.Helpers;
using Gcpe.Hub.API.ViewModels;
using Gcpe.Hub.Data.Entity;
>>>>>>> master
using Newtonsoft.Json;
using Xunit;

namespace Gcpe.Hub.API.IntegrationTests.Views
{
<<<<<<< HEAD
    public class TestMessagesViews : BaseWebApiTest
    {
        public TestMessagesViews(CustomWebApplicationFactory<Startup> factory) : base(factory) {}

        private async Task<Models.Message> _PostMessage(string title = "Lorem Title", int sortOrder = 0)
        {
            var testMessage = TestData.CreateMessage(title, "Lorem description", sortOrder, true, false);
            var createResponse = await Client.PostAsync("/api/messages", testMessage);
            createResponse.EnsureSuccessStatusCode();
            var createBody = await createResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Models.Message>(createBody);
        }

        [Fact]
        public async Task List_EndpointReturnSuccessAndCorrectMessagesSorted()
        {
            for (var i = 0; i < 5; i++)
            {
                await _PostMessage("Sorted Test Message", 5 - i);
            }

            var response = await Client.GetAsync("/api/messages");
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var models = JsonConvert.DeserializeObject<Models.Message[]>(body).Where(m => m.Title == "Sorted Test Message");

            for (int i = 0; i < models.Count() - 1; i++)
            {
                models.ElementAt(i).SortOrder.Should().BeLessThan(models.ElementAt(i + 1).SortOrder);
            }
        }

        [Fact]
        public async Task List_EndpointReturnSuccessAndHandleIfModifiedSince()
        {
            await _PostMessage();

            var response = await Client.GetAsync("/api/messages");
            response.EnsureSuccessStatusCode();

            DateTimeOffset? lastModified = response.Content.Headers.LastModified;
            Client.DefaultRequestHeaders.IfModifiedSince = lastModified;
            response = await Client.GetAsync("/api/messages");
            response.StatusCode.Should().Be(304);
        }

        [Fact]
        public async Task Create_EndpointReturnSuccessAndCorrectMessage()
        {
            var createdMessage = await _PostMessage("Test title!");

            createdMessage.Title.Should().Be("Test title!");
            createdMessage.Description.Should().Be("Lorem description");
=======
    public class MessagesViewsShould : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        public readonly HttpClient _client;
        public MessageViewModel testMessage = MessagesTestData.CreateMessage("Test message title",
            "Test message description", 0);

        public MessagesViewsShould(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task List_EndpointReturnSuccessAndCorrectMessages()
        {
            for (var i = 0; i < 5; i++)
            {
                var newMessage = MessagesTestData.CreateMessage("Test message title", "Test message description", 0);
                var stringContent = new StringContent(JsonConvert.SerializeObject(newMessage), Encoding.UTF8, "application/json");

                var createResponse = await _client.PostAsync("/api/messages", stringContent);
                createResponse.EnsureSuccessStatusCode();
            }

            var response = await _client.GetAsync("/api/messages");
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var deserializedBody = JsonConvert.DeserializeObject<Hub.API.ViewModels.MessageViewModel[]>(body);

            Assert.NotEmpty(deserializedBody);
            deserializedBody.Should().HaveCountGreaterOrEqualTo(5);
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

            messageResult.Title.Should().Be(testMessage.Title);
            messageResult.Description.Should().Be(testMessage.Description);
>>>>>>> master
        }

        [Fact]
        public async Task Create_EndpointRequiresTitle()
        {
<<<<<<< HEAD
            var brokenTestMessage = TestData.CreateMessage(null, "description", 0, true, false);
            var response = await Client.PostAsync("/api/messages", brokenTestMessage);
=======
            testMessage.Id = Guid.Empty;
            var brokenTestMessage = testMessage;
            brokenTestMessage.Title = null;
            var stringContent = new StringContent(JsonConvert.SerializeObject(brokenTestMessage), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/messages", stringContent);
>>>>>>> master
            response.IsSuccessStatusCode.Should().BeFalse();
        }

        [Fact]
        public async Task Create_EndpointDoesntRequireDescription()
        {
<<<<<<< HEAD
            var noDescMessage = TestData.CreateMessage("Title", null, 0, true, false);

            var response = await Client.PostAsync("/api/messages", noDescMessage);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var messageResult = JsonConvert.DeserializeObject<Models.Message>(body);

            messageResult.Title.Should().Be("Title");
=======
            testMessage.Id = Guid.Empty;
            var noDescMessage = testMessage;
            noDescMessage.Description = null;
            var stringContent = new StringContent(JsonConvert.SerializeObject(noDescMessage), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/messages", stringContent);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var messageResult = JsonConvert.DeserializeObject<Hub.API.ViewModels.MessageViewModel>(body);

            messageResult.Title.Should().Be(noDescMessage.Title);
>>>>>>> master
            messageResult.Description.Should().BeNull();
        }

        [Fact]
        public async Task Get_EndpointReturnSuccessAndCorrectMessage()
        {
<<<<<<< HEAD
            Guid id = (await _PostMessage()).Id;

            var response = await Client.GetAsync($"/api/Messages/{id}");
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var messageResult = JsonConvert.DeserializeObject<Models.Message>(body);

=======
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

            messageResult.Title.Should().Be(testMessage.Title);
            messageResult.Description.Should().Be(testMessage.Description);
>>>>>>> master
            messageResult.Id.Should().Be(id);
        }

        [Fact]
<<<<<<< HEAD
        public async Task Get_EndpointReturnsNotFound()
        {
            var response = await Client.GetAsync($"/api/messages/{Guid.NewGuid()}");
=======
        public async Task Get_EndpointNotFound()
        {
            var response = await _client.GetAsync($"/api/messages/{Guid.NewGuid()}");
>>>>>>> master

            response.IsSuccessStatusCode.Should().BeFalse();
        }

        [Fact]
        public async Task Put_EndpointReturnSuccessAndCorrectMessage()
        {
<<<<<<< HEAD
            Guid id = (await _PostMessage()).Id;
            var newTestMessage = TestData.CreateMessage("new title", "new description", 10, true, false);

            var response = await Client.PutAsync($"/api/messages/{id}", newTestMessage);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var messageResult = JsonConvert.DeserializeObject<Models.Message>(body);

            messageResult.Title.Should().Be("new title");
            messageResult.Description.Should().Be("new description");
            messageResult.SortOrder.Should().Be(10);
=======
            var stringContent = new StringContent(JsonConvert.SerializeObject(testMessage), Encoding.UTF8, "application/json");
            var createResponse = await _client.PostAsync("/api/messages", stringContent);
            var createBody = await createResponse.Content.ReadAsStringAsync();
            var createdMessage = JsonConvert.DeserializeObject<Hub.API.ViewModels.MessageViewModel>(createBody);
            var id = createdMessage.Id;

            var newTestMessage = MessagesTestData.CreateMessage("new title", "new description", 10, true, false);
            newTestMessage.Id = Guid.Empty;

            var content = new StringContent(JsonConvert.SerializeObject(newTestMessage), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"/api/messages/{id}", content);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var messageResult = JsonConvert.DeserializeObject<Hub.API.ViewModels.MessageViewModel>(body);

            messageResult.Title.Should().Be(newTestMessage.Title);
            messageResult.Description.Should().Be(newTestMessage.Description);
            messageResult.SortOrder.Should().Be(newTestMessage.SortOrder);
>>>>>>> master
            messageResult.Id.Should().Be(id);
        }

        [Fact]
        public async Task Put_EndpointReturnSuccessWithDefaultsAndCorrectMessage()
        {
<<<<<<< HEAD
            Guid id = (await _PostMessage()).Id;
            var content = new StringContent(JsonConvert.SerializeObject(new { Title = "new title" }), Encoding.UTF8, "application/json");

            var response = await Client.PutAsync($"/api/messages/{id}", content);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var messageResult = JsonConvert.DeserializeObject<Models.Message>(body);

            messageResult.Title.Should().Be("new title");
=======
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

            messageResult.Title.Should().Be(title);
>>>>>>> master
            messageResult.Description.Should().Be(null);
            messageResult.SortOrder.Should().Be(0);
            messageResult.IsPublished.Should().BeFalse();
            messageResult.IsHighlighted.Should().BeFalse();
            messageResult.Id.Should().Be(id);
        }

        [Fact]
        public async Task Put_EndpointShouldRequireTitle()
        {
<<<<<<< HEAD
            Guid id = (await _PostMessage()).Id;
            
            var content = new StringContent(JsonConvert.SerializeObject(new { }), Encoding.UTF8, "application/json");
            var response = await Client.PutAsync($"/api/messages/{id}", content);
=======
            testMessage.Id = Guid.Empty;
            var stringContent = new StringContent(JsonConvert.SerializeObject(testMessage), Encoding.UTF8, "application/json");
            var createResponse = await _client.PostAsync("/api/messages", stringContent);
            var createBody = await createResponse.Content.ReadAsStringAsync();
            var createdMessage = JsonConvert.DeserializeObject<Hub.API.ViewModels.MessageViewModel>(createBody);
            var id = createdMessage.Id;
            
            var content = new StringContent(JsonConvert.SerializeObject(new { }), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"/api/messages/{id}", content);
>>>>>>> master
            response.IsSuccessStatusCode.Should().BeFalse();
        }
    }
}

