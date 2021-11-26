using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CatalogAPIWalkthrough.API.DTOs;
using CatalogAPIWalkthrough.API.Entities;
using CatalogAPIWalkthrough.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CatalogAPIWalkthrough.API.Controllers
{
    [ApiController]
    [Route("[controller]")] // OR [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemsRepository repository;
        private readonly ILogger<ItemsController> logger;

        public ItemsController(IItemsRepository repository, ILogger<ItemsController> logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        //GET /items
        [HttpGet]
        public async Task<IEnumerable<ItemDTO>> GetItemsAsync(string name = null)
        {
            var items = (await repository.GetItemsAsync())
                        .Select(item => item.AsDTO()); //must group await with the get method, not with .Select

            if (!string.IsNullOrWhiteSpace(name)) {
                items = items.Where(item => item.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
            }

            logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")}: Retrieved {items.Count()} items");
                        
            return items;
        }

        //GET /items/{id}
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<ItemDTO>> GetItemAsync(Guid id)
        {
            var item = await repository.GetItemAsync(id);
            if (item is null) return NotFound();
            return item.AsDTO();
        }

        //POST /items
        [HttpPost]
        public async Task<ActionResult<ItemDTO>> CreateItemAsync(CreateItemDTO dto) {
            Item item = new() {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await repository.CreateItemAsync(item);

            return CreatedAtAction(nameof(GetItemAsync), new { id = item.Id }, item.AsDTO());
        }

        //PUT /items/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateItemAsync(Guid id, UpdateItemDTO dto) {
            var existingItem = await repository.GetItemAsync(id);
            if (existingItem is null) return NotFound();

            existingItem.Name = dto.Name;
            existingItem.Price = dto.Price;

            await repository.UpdateItemAsync(existingItem);

            return NoContent();
        }

        //DELETE /items/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteItemAsync(Guid id) {
            if (await repository.GetItemAsync(id) == null) return NotFound();

            await repository.DeleteItemAsync(id);

            return NoContent();
        }

    }
}