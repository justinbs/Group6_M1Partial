using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController(IItemService svc) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Item>>> GetAll() => await svc.GetAllAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Item>> Get(int id)
        => await svc.GetAsync(id) is { } it ? it : NotFound();

    [HttpPost]
    public async Task<ActionResult<Item>> Create(Item item)
    {
        var created = await svc.CreateAsync(item);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Item>> Update(int id, Item item)
        => await svc.UpdateAsync(id, item) is { } it ? it : NotFound();

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
        => await svc.DeleteAsync(id) ? NoContent() : NotFound();
}
