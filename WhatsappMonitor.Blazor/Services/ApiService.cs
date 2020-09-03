using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using System.Text.Json;
using System.Threading.Tasks;
using WhatsappMonitor.Shared.Models;
using Microsoft.AspNetCore.WebUtilities;
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

        public async Task UpdateChatPersonName(int id, ParticipantDTO person)
        {
            var result = await _httpClient.PutAsJsonAsync($"/api/chats/update-name/{id}", person);
        }

        public async Task DeleteChatPersonName(int id, string name)
        {
            var result = await _httpClient.DeleteAsync($"/api/chats/delete-name/{id}/{name}");
        }

        public async Task<List<ChatMessage>> SearchChatWord(int id, string word)
        {
            var result = await _httpClient.GetFromJsonAsync<List<ChatMessage>>($"/api/chats/search/{id}/{word}");
            return result;
        }

        public async Task<Tuple<PaginationDTO, List<ChatMessage>>> LoadChat(int id, int pag, int take)
        {
            var result = await _httpClient.GetFromJsonAsync<Tuple<PaginationDTO, List<ChatMessage>>>($"/api/chats/load/{id}/{pag}/{take}");
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

        public async Task<List<ChatMessage>> LoadChatBefore(int id, DateTime date)
        {
            var l = new List<ChatMessage>();
    
            
            var query = new Dictionary<string, string>
            {
                ["date"] = date.ToString()
            };
            var result = await _httpClient.GetFromJsonAsync<List<ChatMessage>>(QueryHelpers.AddQueryString(($"/api/chats/load-before/{id}"), query));
            return result;
        }

        public async Task<int> JumpChatToDate(int id, DateTime date)
        {
            var query = new Dictionary<string, string>
            {
                ["startDate"] = date.ToString()
            };
            var result = await _httpClient.GetFromJsonAsync<int>(QueryHelpers.AddQueryString(($"/api/chats/search-date/{id}"), query));
            return result;
        }

        public async Task<TotalFolderInfoDTO> GetTotalFolderInfo(int id, DateTime from, DateTime until)
        {
            var query = new Dictionary<string, string>
            {
                ["from"] = from.ToString(),
                ["until"] = until.ToString()
            };

            var result = await _httpClient.GetFromJsonAsync<TotalFolderInfoDTO>(QueryHelpers.AddQueryString(($"/api/chats/chat-info/{id}"), query));
            return result;
        }
    }
}