using System.Net.Http.Json;
using Client.WinForms.Models;

namespace Client.WinForms.Services;

public class ItemApi
{
    private readonly HttpClient _http;
    private const string Base = "http://localhost:5238/api/items"; // ← must match your API http port

    public ItemApi(HttpClient http) => _http = http;

    public Task<List<Item>?> GetAllAsync() => _http.GetFromJsonAsync<List<Item>>(Base);
    public Task<Item?> GetAsync(int id) => _http.GetFromJsonAsync<Item>($"{Base}/{id}");

    public async Task<Item?> CreateAsync(Item item)
    {
        var res = await _http.PostAsJsonAsync(Base, item);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<Item>();
    }

    public async Task<Item?> UpdateAsync(int id, Item item)
    {
        var res = await _http.PutAsJsonAsync($"{Base}/{id}", item);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<Item>();
    }

    public async Task DeleteAsync(int id)
    {
        var res = await _http.DeleteAsync($"{Base}/{id}");
        res.EnsureSuccessStatusCode();
    }
}
