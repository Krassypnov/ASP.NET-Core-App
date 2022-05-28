using CatalogService.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Tests
{
    public abstract class AController
    {
        public AppDbContext dbContext;
        public IEnumerable<Product> productList;
        public IEnumerable<Category> categoriesList;
        public IEnumerable<Brand> brandsList;

        public void InitContext()
        {
            Guid guid = Guid.NewGuid();
            var builder = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(guid.ToString());

            productList = GetTestProducts();
            categoriesList = GetTestCategories();
            brandsList = GetTestBrands();

            var context = new AppDbContext(builder.Options);

            context.Categories.AddRange(categoriesList);
            context.Products.AddRange(productList);
            context.Brands.AddRange(brandsList);
            context.SaveChanges();

            dbContext = context;
        }

        public IList<Product> GetTestProducts()
        {
            List<Product> products = new List<Product>();
            for (int i = 0; i < 25; i++)
                products.Add(new Product
                {
                    Id = Guid.NewGuid(),
                    Brand = $"Brand {(i / 5)}",
                    Category = $"Category {(i / 5)}",
                    Count = i + 1,
                    Price = (i + 1) * 1000,
                    ProductName = $"ProductName {i}"
                });
            return products;
        }

        public IList<Brand> GetTestBrands()
        {
            var brands = new List<Brand>();

            for (int i = 0; i < 5; i++)
            {
                brands.Add(new Brand { Id = Guid.NewGuid(), BrandName = $"Brand {i}" });
            }

            return brands;
        }

        public IList<Category> GetTestCategories()
        {
            var categories = new List<Category>();

            for (int i = 0; i < 5; i++)
            {
                categories.Add(new Category { Id = Guid.NewGuid(), CategoryName = $"Category {i}" });
            }

            return categories;
        }
    }
}
