using CatalogAPIWalkthrough.API.DTOs;
using CatalogAPIWalkthrough.API.Entities;

namespace CatalogAPIWalkthrough.API
{
    public static class Extensions
    {
        public static ItemDTO AsDTO(this Item item)
        {
            return new ItemDTO
            {
                Id = item.Id,
                Name = item.Name,
                Price = item.Price,
                CreatedDate = item.CreatedDate
            };
        }
    }
}