using System;
using System.Collections.Generic;
using CatalogAPIWalkthrough.Entities;

namespace CatalogAPIWalkthrough.Repositories
{
    public interface IItemsRepository
    {
        Item GetItem(Guid id);
        IEnumerable<Item> GetItems();
        void CreateItem(Item item);
        void UpdateItem(Item item);
    }
}