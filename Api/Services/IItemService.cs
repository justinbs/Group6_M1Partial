using Api.Models;

namespace Api.Services;

public interface IItemService
{
    Task<List<Item>> GetAllAsync();
    Task<Item?> GetAsync(int id);
    Task<Item> CreateAsync(Item item);
    Task<Item?> UpdateAsync(int id, Item item);
    Task<bool> DeleteAsync(int id);
}
