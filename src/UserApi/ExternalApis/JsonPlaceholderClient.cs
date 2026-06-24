using System.Net.Http.Json;
using System.Net;
using UserApi.Models;

namespace UserApi.ExternalApis;
public class JsonPlaceholderClient
{
    private readonly HttpClient _httpClient;
    public JsonPlaceholderClient(HttpClient httpClient)
    {
        _httpClient= httpClient;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        
        using var response = await _httpClient.GetAsync("users", HttpCompletionOption.ResponseHeadersRead);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return new List<User>();
        }
        response.EnsureSuccessStatusCode();
        var users = await response.Content.ReadFromJsonAsync<List<User>>();
        return users ?? new List<User>();
    }
    public async Task<User?>GetUserByIdAsync(int id)
    {
        using var response = await _httpClient.GetAsync($"users/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<User>();
    }

}