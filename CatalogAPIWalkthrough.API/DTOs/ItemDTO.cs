using System;

namespace CatalogAPIWalkthrough.API.DTOs
{
    public record ItemDTO
    {
        public Guid Id { get; init; } //init is immutable after instantiation; kind of like private set but doesn't need constructor
        public string Name { get; init; }
        public decimal Price { get; init; }
        public DateTimeOffset CreatedDate { get; init; } 
    }
}