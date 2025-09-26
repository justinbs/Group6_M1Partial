using Api.Data;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public class ItemService(AppDbContext db) : IItemService
{
    public Task<List<Item>> GetAllAsync() => db.Items.OrderBy(i => i.Name).ToListAsync();

    public Task<Item?> GetAsync(int id) => db.Items.FindAsync(id).AsTask();

    public async Task<Item> CreateAsync(Item item)
    {
        db.Items.Add(item);
        await db.SaveChangesAsync();
        return item;
    }

    public async Task<Item?> UpdateAsync(int id, Item item)
    {
        var existing = await db.Items.FindAsync(id);
        if (existing is null) return null;

        existing.Name = item.Name;
        existing.Code = item.Code;
        existing.Brand = item.Brand;
        existing.UnitPrice = item.UnitPrice;

        await db.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await db.Items.FindAsync(id);
        if (existing is null) return false;
        db.Items.Remove(existing);
        await db.SaveChangesAsync();
        return true;
    }
}
