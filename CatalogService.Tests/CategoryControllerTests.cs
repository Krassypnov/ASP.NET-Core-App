using Microsoft.AspNetCore.Mvc;
using CatalogService.Controllers;
using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using CatalogService.Models;

namespace CatalogService.Tests
{
    public class CategoryControllerTests : AController
    {

        public CategoryControllerTests()
        {
            InitContext();
        }


        [Fact]
        public void GetCategories_ReturnsListOfCategories()
        {
            // Arrange

            CategoryController controller = new CategoryController(dbContext);

            // Act
            
            var result = controller.getCategories();

            // Assert
            var returnResult = Assert.IsType<JsonResult>(result);
            var categories = (IEnumerable<Category>)returnResult.Value;
            Assert.Equal(GetTestCategories().Count, categories.Count());
        }

        [Fact]
        public void GetCategory_ReturnCategoryWithId()
        {
            // Arrange

            CategoryController controller = new CategoryController(dbContext);
            Guid correctId = categoriesList.First().Id;

            // Act

            var result = controller.getCategory(correctId);

            // Assert

            var firstObject = (Category)Assert.IsType<JsonResult>(result).Value;
            Assert.Equal(categoriesList.First().Id, firstObject.Id);
        }

        [Fact]
        public void AddCategory_ReturnsOk_IfSuccessfulAdd()
        {
            // Arrange 

            var controller = new CategoryController(dbContext);

            // Act 

            var result = controller.addCategory("CategoryName");

            // Assert

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void DeleteCategoryByName_ReturnsOkOrNotFound()
        {
            // Arrange

            var controller = new CategoryController(dbContext);

            // Act

            var okResult = controller.deleteCategoryByName("Category 0");
            var notFoundResult = controller.deleteCategoryByName("WrongCategoryName");

            // Assert

            Assert.IsType<OkObjectResult>(okResult);
            Assert.IsType<NotFoundResult>(notFoundResult);
        }

        [Fact]
        public void DeleteCategoryById_ReturnsOkOrNotFound()
        {
            // Arrange

            var controller = new CategoryController(dbContext);
            Guid wrongId = Guid.NewGuid();

            // Act

            var okResult = controller.deleteCategoryById(categoriesList.First().Id);
            var notFoundResult = controller.deleteCategoryById(wrongId);

            // Assert

            Assert.IsType<OkObjectResult>(okResult);
            Assert.IsType<NotFoundResult>(notFoundResult);
        }


        [Fact]
        public void UpdateCategory_ReturnsOk_IfSuccessfulRequest()
        {
            // Arrange

            var controller = new CategoryController(dbContext);
            var oldCategoryName = categoriesList.First().CategoryName;
            var newCategoryName = "New Category Name";

            // Act

            var result = controller.updateCategory(oldCategoryName, newCategoryName);

            // Assert

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void UpdateCategory_ReturnsBadRequest_IfWrongCategoryName()
        {
            // Arrange

            var controller = new CategoryController(dbContext);
            var wrongCategoryName = "Wrong Name";

            // Act

            var result = controller.updateCategory(wrongCategoryName, "CategoryName");

            // Assert

            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public void UpdateCategory_ReturnsBadRequest_IfCategoryNameAlreadyExist()
        {
            // Arrange

            var controller = new CategoryController(dbContext);
            var oldCategoryName = categoriesList.First().CategoryName;
            var newCategoryName = categoriesList.ElementAt(1).CategoryName;

            // Act

            var result = controller.updateCategory(oldCategoryName, newCategoryName);

            // Assert

            Assert.IsType<BadRequestObjectResult>(result);
        }

    }
}
