using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CatalogAPIWalkthrough.API.Controllers;
using CatalogAPIWalkthrough.API.DTOs;
using CatalogAPIWalkthrough.API.Entities;
using CatalogAPIWalkthrough.API.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CatalogAPIWalkthrough.UnitTests
{
    public class ItemsControllerTests
    {
        private readonly Mock<IItemsRepository> repositoryStub = new();

        private readonly Mock<ILogger<ItemsController>> loggerStub = new();

        private readonly Random rand = new();

        [Fact]
        //unit test naming convention: UnitOfWork_StateUnderTest_ExpectedBehavior
        public async Task GetItemAsync_WithNonexistentItem_ReturnsNotFound()
        {
            //Arrange
            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Item)null);

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            //Act
            var result = await controller.GetItemAsync(Guid.NewGuid());

            //Assert
            //Assert.IsType<NotFoundResult>(result.Result);
            //refactor to use Fluent Assertions library
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        //unit test naming convention: UnitOfWork_StateUnderTest_ExpectedBehavior
        public async Task GetItemAsync_WithExistingItem_ReturnsExpectedItem()
        {
            //Arrange
            var expectedItem = CreateRandomItem();

            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedItem);

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            //Act
            var result = await controller.GetItemAsync(Guid.NewGuid());

            //Assert

            //result.Value.Should().BeEquivalentTo(expectedItem);
            // the above assertion is standard but doesn't work completely in our case because we are comparing DTO to Record type... or something like that...
            // the below overloaded method compares the properties of each object, not the objects themselves... or something like that...

            // result.Value.Should().BeEquivalentTo(
            //     expectedItem,
            //     options => options.ComparingByMembers<Item>());

            //below is not best practice, use "FLUENT ASSERTIONS" nuget package instead (above)
            // Assert.IsType<ItemDTO>(result.Value);
            // var dto = (result as ActionResult<ItemDTO>).Value;
            // Assert.Equal(expectedItem.Id, dto.Id);
            // Assert.Equal(expectedItem.Name, dto.Name);
            // ...assert the rest of the properties...

            //after refactoring Item to be class instead of record type, we no longer need the "options" and can use the more typical assertion below
            result.Value.Should().BeEquivalentTo(expectedItem);
        }

        [Fact]
        public async Task GetItemsAsync_WithExistingItems_ReturnsAllItems()
        {
            //Arrange
            var expectedItems = new[] { CreateRandomItem(), CreateRandomItem(), CreateRandomItem() };

            repositoryStub.Setup(repo => repo.GetItemsAsync())
                .ReturnsAsync(expectedItems);

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            //Act
            var actualItems = await controller.GetItemsAsync();

            //Assert
            actualItems.Should().BeEquivalentTo(expectedItems);
        }

        [Fact]
        public async Task GetItemsAsync_WithMatchingItems_ReturnsMatchingItems()
        {
            //Arrange
            var allItems = new[] 
            {
                new Item(){ Name = "Potion"},
                new Item(){ Name = "Antidote"},
                new Item(){ Name = "Hi-Potion"}
            };

            var nameToMatch = "Potion";

            repositoryStub.Setup(repo => repo.GetItemsAsync())
                .ReturnsAsync(allItems);

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            //Act
            IEnumerable<ItemDTO> foundItems = await controller.GetItemsAsync(nameToMatch);
            
            //Assert
            foundItems.Should().OnlyContain(
                item => item.Name == allItems[0].Name || item.Name == allItems[2].Name
            );
        }

        [Fact]
        public async Task CreateItemAsync_WithItemToCreate_ReturnsCreatedItem()
        {
            //Arrange
            var itemToCreate = new CreateItemDTO(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                rand.Next(1000)
                );

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            //Act
            var result = await controller.CreateItemAsync(itemToCreate);

            //Assert
            var createdItem = (result.Result as CreatedAtActionResult).Value as ItemDTO;
            itemToCreate.Should().BeEquivalentTo(
                createdItem,
                options => options.ComparingByMembers<ItemDTO>().ExcludingMissingMembers()
            );
            createdItem.Id.Should().NotBeEmpty();
            createdItem.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromMilliseconds(1000));
        }


        [Fact]
        public async Task UpdateItemAsync_WithExistingItem_ReturnsNoContent()
        {
            //Arrange
            var existingItem = CreateRandomItem();

            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingItem);

            var itemId = existingItem.Id;

            var itemToUpdate = new UpdateItemDTO(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                existingItem.Price + 3
            );

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            //Act
            var result = await controller.UpdateItemAsync(itemId, itemToUpdate);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }


        [Fact]
        public async Task DeleteItemAsync_WithExistingItem_ReturnsNoContent()
        {
            //Arrange
            var existingItem = CreateRandomItem();

            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingItem);

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            //Act
            var result = await controller.DeleteItemAsync(existingItem.Id);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }


        private Item CreateRandomItem()
        {
            return new()
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                Price = rand.Next(1000),
                CreatedDate = DateTimeOffset.UtcNow
            };
        }
    }
}
