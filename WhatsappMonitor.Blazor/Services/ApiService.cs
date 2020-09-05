using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using WhatsappMonitor.Shared.Models;
using Microsoft.AspNetCore.WebUtilities;
using System;

namespace WhatsappMonitor.Blazor.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        public ApiService(HttpClient client)
        {
            _httpClient = client;
        }

        public async Task<List<Folder>> GetentitiesAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<Folder>>("api/folders");
            return response;
        }

        public async Task<Folder> GetEntityByIdAsync(string id)
        {
            var response = await _httpClient.GetFromJsonAsync<Folder>($"api/folders/{id}");
            return response;
        }

        public async Task<List<Folder>> DeleteEntityById(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/folders/{id}");
            return await response.Content.ReadFromJsonAsync<List<Folder>>();
        }

        public async Task<List<Folder>> EditEntity(Folder Folder)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/folders/{Folder.FolderId}", Folder);
            return await response.Content.ReadFromJsonAsync<List<Folder>>();
        }

        public async Task<List<Folder>> AddEntity(string Entityname)
        {
            var Folder = new Folder(Entityname);
            var response = await _httpClient.PostAsJsonAsync($"api/folders", Folder);
            return await response.Content.ReadFromJsonAsync<List<Folder>>();
        }

        public async Task<int> PostFile(MultipartFormDataContent file, int id)
        {
            var result = await _httpClient.PostAsync($"/api/folders/file/{id}", file);
            return await result.Content.ReadFromJsonAsync<int>();
        }

        public async Task<List<ParticipantDTO>> GetParticipants(int id)
        {
            var result = await _httpClient.GetFromJsonAsync<List<ParticipantDTO>>($"/api/chats/chat-participants/{id}");
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

        public async Task<List<ParticipantDTO>> UpdateChatParticipants(int id, List<ParticipantDTO> participants)
        {
            var result = await _httpClient.PutAsJsonAsync($"/api/chats/update-participants/{id}", participants);

            return await result.Content.ReadFromJsonAsync<List<ParticipantDTO>>();
        }

        public async Task<List<ChatMessage>> SearchChatWord(int id, string word)
        {
            var result = await _httpClient.GetFromJsonAsync<List<ChatMessage>>($"/api/chats/search/{id}/{word}");
            return result;
        }

        public async Task<List<ChatMessage>> LoadChatBefore(int id, DateTime date)
        {
            var query = new Dictionary<string, string>
            {
                ["date"] = date.ToString()
            };
            var result = await _httpClient.GetFromJsonAsync<List<ChatMessage>>(QueryHelpers.AddQueryString(($"/api/chats/load-before/{id}"), query));
            return result;
        }

        public async Task<List<ChatMessage>> GoToFirstMessage(int id)
        {
            var result = await _httpClient.GetFromJsonAsync<List<ChatMessage>>($"/api/chats/first-message/{id}");
            return result;
        }

        public async Task<List<ChatMessage>> GoToLastMessage(int id)
        {
            var result = await _httpClient.GetFromJsonAsync<List<ChatMessage>>($"/api/chats/last-message/{id}");
            return result;
        }

        public async Task<List<ChatMessage>> LoadChatAfter(int id, DateTime date)
        {
            var query = new Dictionary<string, string>
            {
                ["date"] = date.ToString()
            };
            var result = await _httpClient.GetFromJsonAsync<List<ChatMessage>>(QueryHelpers.AddQueryString(($"/api/chats/load-after/{id}"), query));
            return result;
        }

        public async Task<List<ChatMessage>> JumpChatToDate(int id, DateTime date)
        {
            var query = new Dictionary<string, string>
            {
                ["startDate"] = date.ToString()
            };
            var result = await _httpClient.GetFromJsonAsync<List<ChatMessage>>(QueryHelpers.AddQueryString(($"/api/chats/jump-date/{id}"), query));
            return result;
        }
    }
}