using System;
using System.Collections.Generic;
using System.Linq;
using CatalogAPIWalkthrough.DTOs;
using CatalogAPIWalkthrough.Entities;
using CatalogAPIWalkthrough.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CatalogAPIWalkthrough.Controllers
{
    [ApiController]
    [Route("[controller]")] // OR [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemsRepository repository;

        public ItemsController(IItemsRepository repository)
        {
            this.repository = repository;
        }

        //GET /items
        [HttpGet]
        public IEnumerable<ItemDTO> GetItems()
        {
            var items = repository.GetItems().Select(item => item.AsDTO());
            return items;
        }

        //GET /items/id
        [HttpGet]
        [Route("{id}")]
        public ActionResult<ItemDTO> GetItem(Guid id)
        {
            var item = repository.GetItem(id);
            if (item is null) return NotFound();
            return item.AsDTO();
        }

        //POST /items
        [HttpPost]
        public ActionResult<ItemDTO> CreateItem(CreateItemDTO dto) {
            Item item = new() {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Price = dto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };

            repository.CreateItem(item);

            return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item.AsDTO());
        }
    }
}