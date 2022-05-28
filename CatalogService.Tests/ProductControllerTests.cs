using CatalogService.Controllers;
using CatalogService.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace CatalogService.Tests
{
    public class ProductControllerTests : AController
    {
        public ProductControllerTests()
        {
            InitContext();
        }

        [Fact]
        public void GetProducts_ReturnsListOfProducts()
        {
            // Arrange

            var controller = new ProductController(dbContext);

            // Act

            var result = controller.GetProducts();

            // Assert

            var listObjects = (IEnumerable<Product>)Assert.IsType<JsonResult>(result).Value;
            Assert.Equal(productList.Count(), listObjects.Count());
        }

        [Fact]
        public void GetProductById_ReturnsProductWithId()
        {
            // Arrange

            var controller = new ProductController(dbContext);
            Guid correctId = productList.First().Id;

            // Act

            var result = controller.GetProductById(correctId);

            // Assert

            var objResult = (Product)Assert.IsType<JsonResult>(result).Value;
            Assert.Equal(correctId, objResult.Id);
        }

        [Fact]
        public void GetProductById_ReturnsNotFound_IfIdIncorrect()
        {
            // Arrange

            var controller = new ProductController(dbContext);
            Guid incorrectId = Guid.NewGuid();

            // Act

            var result = controller.GetProductById(incorrectId);

            // Assert

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void GetProductByName_ReturnsProductWithId()
        {
            // Arrange

            var controller = new ProductController(dbContext);
            var correctName = productList.First().ProductName;

            // Act

            var result = controller.GetProductByName(correctName);

            // Assert

            var objResult = (Product)Assert.IsType<JsonResult>(result).Value;
            Assert.Equal(correctName, objResult.ProductName);
        }

        [Fact]
        public void GetProductByName_ReturnsNotFound_IfNameIncorrect()
        {
            // Arrange

            var controller = new ProductController(dbContext);
            var incorrectName = "Name";

            // Act

            var result = controller.GetProductByName(incorrectName);

            // Assert

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void GetProductByCategory_ReturnsProductsList_IfCategoryExists()
        {
            // Arrange

            var controller = new ProductController(dbContext);
            var correctCategory = productList.First().Category;

            // Act

            var result = controller.GetProductsByCategory(correctCategory);

            // Assert

            var objResult = (IEnumerable<Product>)Assert.IsType<JsonResult>(result).Value;
            Assert.Equal(productList.Where(c => c.Category == correctCategory).Count(), objResult.Count());
            foreach (var product in objResult)
                Assert.Equal(correctCategory, product.Category);
        }

        [Fact]
        public void GetProductByCategory_ReturnsNotFound_IfCategoryNotExists()
        {
            // Arrange

            var controller = new ProductController(dbContext);
            var incorrectCategory = "NotCategory";

            // Act

            var result = controller.GetProductsByCategory(incorrectCategory);

            // Assert

            Assert.IsType<NotFoundResult>(result);
        }
        
        [Fact]
        public void GetProductsByBrand_ReturnsProductsList_IfBrandExists()
        {
            // Arrange

            var controller = new ProductController(dbContext);
            var correctBrand = productList.First().Brand;

            // Act

            var result = controller.GetProductsByBrand(correctBrand);

            // Assert

            var objResult = (IEnumerable<Product>)Assert.IsType<JsonResult>(result).Value;
            Assert.Equal(productList.Where(c => c.Brand == correctBrand).Count(), objResult.Count());
            
            foreach (var product in objResult)
                Assert.Equal(correctBrand, product.Brand);
        }

        [Fact]
        public void GetProductsByBrand_ReturnsNotFound_IfBrandNotExists()
        {
            // Arrange

            var controller = new ProductController(dbContext);
            var incorrectBrand = "NotBrand";

            // Act

            var result = controller.GetProductsByBrand(incorrectBrand);

            // Assert

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void ChangeCount_ReturnsOk_IfCorrectProductNameAndCount()
        {
            // Arrange

            var controller = new ProductController(dbContext);
            var correctProductName = productList.First().ProductName;
            var correctCount = 5;

            // Act

            var result = controller.ChangeCount(correctProductName, correctCount);

            // Assert

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void ChangeCount_ReturnsBadRequest_IfIncorrectProductName()
        {
            // Arrange

            var controller = new ProductController(dbContext);
            var incorrectProductName = "NotProduct";
            var correctCount = 5;

            // Act

            var result = controller.ChangeCount(incorrectProductName, correctCount);

            // Assert

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void ChangeCount_ReturnsBadRequest_IfIncorrectCount()
        {
            // Arrange

            var controller = new ProductController(dbContext);
            var correctProductName = productList.First().ProductName;
            var incorrectCount = -5;

            // Act

            var result = controller.ChangeCount(correctProductName, incorrectCount);

            // Assert

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
