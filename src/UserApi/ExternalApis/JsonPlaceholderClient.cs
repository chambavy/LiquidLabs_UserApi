using System.Net.Http.Json;
using UserApi.Models;

namespace UserApi.ExternalApis;
public class JsonPlaceHolderClient
{
    private readonly HttpClient _httpClient;
    public JsonPlaceHolderClient(HttpClient httpClient)
    {
        _httpClient= httpClient;
    }
    
    public async Task<List<User>>GetAllUsersAsync()
    {
        //using HttpResponseMessage response = await _httpClient.GetAsync("users");
        var users = new List<User>();
        //response.EnsureSuccessStatusCode()
        //    .WriteRequestToConsole();
        var baseUrl = "https://jsonplaceholder.typicode.com/";
        users = await _httpClient.GetFromJsonAsync<List<User>>($"{baseUrl}users");
        if (users.Count == 0)
            throw new Exception("Users not found");

        return users;

    }
    public async Task<User>GetUserByIdAsync(int id)
    {
        //using HttpResponseMessage response = await _httpClient.GetAsync("users/{id}");

        //response.EnsureSuccessStatusCode()
        //    .WriteRequestToConsole();
        var baseUrl = "https://jsonplaceholder.typicode.com/";
        var user = await _httpClient.GetFromJsonAsync<User>($"{baseUrl}users/{id}");
        if (user == null)
            throw new Exception("User not found");

        return user;

    }

}