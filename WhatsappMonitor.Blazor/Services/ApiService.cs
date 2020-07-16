using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using System.Text.Json;
using System.Threading.Tasks;
using WhatsappMonitor.Shared.Models;
using System.IO;
 
namespace WhatsappMonitor.Blazor.Services
{
    public class ApiService
    {
        private readonly IWebHostEnvironment _env;
        private readonly HttpClient _httpClient;
        public ApiService(HttpClient client)
        {
            _httpClient = client;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<User>>("api/users");
            return response;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            var response = await _httpClient.GetFromJsonAsync<User>($"api/users/{id}");
            return response;
        }

        public async Task DeleteUserById(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/users/{id}");
        }

        public async Task EditUser(User user)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/users/{user.Id}", user);
        }

        public async Task AddUser(string username)
        {
            var user = new User { Name = username };

            var response = await _httpClient.PostAsJsonAsync($"api/users", user);
        }

        //Groups
        public async Task<List<Group>> GetGroupsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<Group>>("api/groups");
            return response;
        }

        public async Task<Group> GetGroupByIdAsync(int id)
        {
            var response = await _httpClient.GetFromJsonAsync<Group>($"api/groups/{id}");
            return response;
        }

        public async Task DeleteGroupById(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/groups/{id}");
        }

        public async Task EditGroup(Group group)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/groups/{group.Id}", group);
        }

        public async Task AddGroup(string groupname)
        {
            var group = new Group { Name = groupname };
            var response = await _httpClient.PostAsJsonAsync($"api/groups", group);
        }

        public async Task PostFile(MultipartFormDataContent file, int id)
        {
            await _httpClient.PostAsync($"/api/groups/file/{id}", file);
        }

      
    }
}