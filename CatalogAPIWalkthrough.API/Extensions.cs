using CatalogAPIWalkthrough.API.DTOs;
using CatalogAPIWalkthrough.API.Entities;

namespace CatalogAPIWalkthrough.API
{
    public static class Extensions
    {
        public static ItemDTO AsDTO(this Item item)
        {
            return new ItemDTO(item.Id, item.Name, item.Description, item.Price, item.CreatedDate);
        }
    }
}