using Microsoft.AspNetCore.Mvc;
using CatalogService.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace CatalogService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly AppDbContext _db;

        public ProductController(AppDbContext db)
        {
            _db = db;
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpGet]
        public ActionResult getProducts()
        {
            var obj = _db.Products;
            
            if (obj == null)
                return NotFound();
            return Json(obj);
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpGet("getProductById/{id}")]
        public ActionResult getProductById(Guid id)
        {
            var obj = _db.Products.FirstOrDefault(c => c.Id == id);
            if (obj == null)
                return NotFound();

            return Json(obj);
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpGet("getProductByName/{name}")]
        public ActionResult getProductById(string name)
        {
            var obj = _db.Products.FirstOrDefault(c => c.ProductName == name);
            if (obj == null)
                return NotFound();

            return Json(obj);
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpGet("getProductsByCategory/{name}")]
        public ActionResult getProductsByCategory(string name)
        {
            var obj = _db.Products.Where(c => c.Category == name);
            if (obj == null)
                return NotFound();

            return Json(obj);
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpGet("getProductsByBrand/{name}")]
        public ActionResult getProductsByBrand(string name)
        {
            var obj = _db.Products.Where(c => c.Brand == name);
            if (obj == null)
                return NotFound();

            return Json(obj);
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [HttpPut("changeCount/{name},{difference}")]
        public ActionResult changeCount(string name, int difference)
        {
            var product = _db.Products.FirstOrDefault(c => c.ProductName == name);
            if (product == null)
                return BadRequest("Product not found");

            product.Count += difference;
            if (product.Count < 0)
                return BadRequest("The difference must be less than or equal to the count");

            _db.Products.Update(product);
            _db.SaveChanges();
            return Ok("Count successfully updated");
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [HttpPost("addProduct/{name},{category},{brand},{count}")]
        public ActionResult addProduct(string name, string category, string brand, int count = 0)
        {
            var product = _db.Products.FirstOrDefault(c => c.ProductName == name);
            if (product != null)
                return BadRequest("Product already exists");

            var objCategory = _db.Categories.FirstOrDefault(c => c.CategoryName == category);
            if (objCategory == null)
                return BadRequest("This product category does not exist");
            var objBrand = _db.Brands.FirstOrDefault(c => c.BrandName == brand);
            if (objBrand == null)
                return BadRequest("This brand does not exist");
            if (count < 0)
                return BadRequest("Count must be greater than or equal to 0");

            _db.Products.Add(new Product { ProductName = name, Category = category, Brand = brand, Count = count });
            _db.SaveChanges();

            return Ok("Product added successfully");
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpDelete("deleteProductByName/{name}")]
        public ActionResult deleteProductByName(string name)
        {
            var obj = _db.Products.FirstOrDefault(c => c.ProductName == name);
            if (obj == null)
                return NotFound();

            _db.Products.Remove(obj);
            _db.SaveChanges();
            return Ok("Product successfully deleted");
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpDelete("deleteProductById/{id}")]
        public ActionResult deleteProductByName(Guid id)
        {
            var obj = _db.Products.FirstOrDefault(c => c.Id == id);
            if (obj == null)
                return NotFound();

            _db.Products.Remove(obj);
            _db.SaveChanges();
            return Ok("Product successfully deleted");
        }
    }
}
