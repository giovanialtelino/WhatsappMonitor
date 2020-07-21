using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using System.Text.Json;
using System.Threading.Tasks;
using WhatsappMonitor.Shared.Models;
using System.IO;
using System;

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

        public async Task<List<Entity>> GetentitiesAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<Entity>>("api/entities");
            return response;
        }

        public async Task<Entity> GetEntityByIdAsync(string id)
        {
            var response = await _httpClient.GetFromJsonAsync<Entity>($"api/entities/{id}");
            return response;
        }

        public async Task DeleteEntityById(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/entities/{id}");
        }

        public async Task EditEntity(Entity Entity)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/entities/{Entity.EntityId}", Entity);
        }

        public async Task AddEntity(string Entityname)
        {
            var Entity = new Entity(Entityname);
            var response = await _httpClient.PostAsJsonAsync($"api/entities", Entity);
        }

        public async Task<int> PostFile(MultipartFormDataContent file, int id)
        {
            var result = await _httpClient.PostAsync($"/api/entities/file/{id}", file);
            return await result.Content.ReadFromJsonAsync<int>();
        }

        public async Task<List<Chat>> GetChatsEntity(int id)
        {
            var result = await _httpClient.GetFromJsonAsync<List<Chat>>($"/api/chats/{id}");
            return result;
        }

        public async Task<List<ParticipantDTO>> GetParticipants(int id)
        {
            var result = await _httpClient.GetFromJsonAsync<List<ParticipantDTO>>($"/api/chats/members/{id}");
            return result;
        }

        public async Task<List<ChatUploadDTO>> GetChatUploadDates(int id)
        {
            var result = await _httpClient.GetFromJsonAsync<List<ChatUploadDTO>>($"/api/chats/uploads/{id}");

            return result;
        }

        public async Task DeleteChatUpload(int id, ChatUploadDTO toDelete)
        {
            var result = await _httpClient.PutAsJsonAsync($"/api/chats/delete-date/{id}", toDelete);
        }

        public async Task UpdateChatPersonName(int id, ParticipantDTO person)
        {
            var result = await _httpClient.PutAsJsonAsync($"/api/chats/update-name/{id}", person);
        }

        public async Task DeleteChatPersonName(int id, string name)
        {            
            var result = await _httpClient.DeleteAsync($"/api/chats/delete-name/{id}/{name}");
        }

         public async Task<List<Chat>> SearchChatWord(int id, string word)
        {            
            var result = await _httpClient.GetFromJsonAsync<List<Chat>>($"/api/chats/search/{id}/{word}");
            return result;
        }

        public async Task<List<Chat>> LoadChat(int id, int pag)
        {
            var result = await _httpClient.GetFromJsonAsync<List<Chat>>($"/api/chats/load/{id}/{pag}");
            return result;
        }
    }
}