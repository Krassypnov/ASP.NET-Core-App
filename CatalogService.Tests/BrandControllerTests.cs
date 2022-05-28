using Microsoft.AspNetCore.Mvc;
using CatalogService.Controllers;
using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using CatalogService.Models;


namespace CatalogService.Tests
{
    public class BrandControllerTests : AController
    {
        public BrandControllerTests()
        {
            InitContext();
        }


        [Fact]
        public void GetBrands_ReturnsListOfBrands()
        {
            // Arrange

            BrandController controller = new BrandController(dbContext);

            // Act
            
            var result = controller.GetBrands();

            // Assert
            var returnResult = Assert.IsType<JsonResult>(result);
            var brands = (IEnumerable<Brand>)returnResult.Value;
            Assert.Equal(GetTestBrands().Count, brands.Count());

        }

        [Fact]
        public void GetBrand_ReturnBrandWithId()
        {
            // Arrange

            BrandController controller = new BrandController(dbContext);

            // Act

            foreach (var brand in brandsList)
            {
                var result = controller.GetBrand(brand.Id);
                
                // Assert
                var returnResult = Assert.IsType<JsonResult>(result);
                var brandFromController = (Brand)returnResult.Value;
                Assert.Equal(brand.Id, brandFromController.Id);
            }

        }

        [Fact]
        public void AddBrand_ReturnsOk_IfSuccessfulAdd()
        {
            // Arrange 

            var controller = new BrandController(dbContext);

            // Act 

            var result = controller.AddBrand("BrandName");

            // Assert

            Assert.IsType<OkResult>(result);

        }

        [Fact]
        public void DeleteBrandByName_ReturnsOk_IfSuccessfulDelete()
        {
            // Arrange

            var controller = new BrandController(dbContext);

            // Act
            foreach (var brand in brandsList)
            {
                var result = controller.DeleteBrandByName(brand.BrandName);

                // Assert

                Assert.IsType<OkObjectResult>(result);
            }
            
        }

        [Fact]
        public void DeleteBrandById_ReturnsOk_IfSuccessfulDelete()
        {
            // Arrange

            var controller = new BrandController(dbContext);

            // Act
            foreach (var brand in brandsList)
            {
                var result = controller.DeleteBrandById(brand.Id);

                // Assert

                Assert.IsType<OkObjectResult>(result);
            }

        }

        
    }
}
