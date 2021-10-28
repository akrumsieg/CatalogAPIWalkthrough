using System;
using System.Collections.Generic;
using CatalogAPIWalkthrough.Entities;
using CatalogAPIWalkthrough.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CatalogAPIWalkthrough.Controllers
{
    [ApiController]
    [Route("[controller]")] // OR [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly InMemItemsRepository repository;

        public ItemsController()
        {
            repository = new InMemItemsRepository();
        }

        //GET /items
        [HttpGet]
        public IEnumerable<Item> GetItems()
        {
            var items = repository.GetItems();
            return items;
        }

        //GET /items/id
        [HttpGet]
        [Route("{id}")]
        public ActionResult GetItem(Guid id) {
            var item = repository.GetItem(id);
            if (item is null) return NotFound();
            return Ok(item);
        }
    }
}