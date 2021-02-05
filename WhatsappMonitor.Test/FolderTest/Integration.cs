using System;
using Xunit;
using Xunit.Priority;
using WhatsappMonitor.API.Helper;
using System.Collections.Generic;
using WhatsappMonitor.Shared.Models;
using WhatsappMonitor.Test;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft;
using Newtonsoft.Json.Serialization;
using System.Text;
using System.Net.Http;

namespace WhatsappMonitor.Test.FolderTest
{
    public class Integration : IClassFixture<CustomWebApplicationFactory<WhatsappMonitor.API.Startup>>
    {
        private readonly CustomWebApplicationFactory<WhatsappMonitor.API.Startup> _factory;

        public Integration(CustomWebApplicationFactory<WhatsappMonitor.API.Startup> factory)
        {
            _factory = factory;
        }

        [Fact, Priority(0)]
        public async Task GetAllFolders()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/api/folders");
            var result = await response.Content.ReadAsStringAsync();
            var jsonResult = JsonConvert.DeserializeObject<List<Folder>>(result);

            //was just created
            Assert.Equal(jsonResult.Count, 0);
        }

        [Fact, Priority(1)]
        public async Task AddFolder()
        {
            var newFolder = new Folder("testFolder");

            var addMessageJson = new StringContent(
              JsonConvert.SerializeObject(newFolder),
              Encoding.UTF8,
              "application/json");

            var client = _factory.CreateClient();
            var response = await client.PostAsync("/api/folders", addMessageJson);
            var result = await response.Content.ReadAsStringAsync();
            var jsonResult = JsonConvert.DeserializeObject<List<Folder>>(result);

            //was just created
            Assert.Equal(jsonResult.Count, 1);
        }
    }
}
