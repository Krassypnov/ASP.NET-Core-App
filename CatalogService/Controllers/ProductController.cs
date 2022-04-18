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
        [HttpGet("getById/{id}")]
        public ActionResult getProductById(Guid id)
        {
            var obj = _db.Products.FirstOrDefault(c => c.Id == id);
            if (obj == null)
                return NotFound();

            return Json(obj);
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpGet("getByName/{name}")]
        public ActionResult getProductById(string name)
        {
            var obj = _db.Products.FirstOrDefault(c => c.ProductName == name);
            if (obj == null)
                return NotFound();

            return Json(obj);
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [HttpPost("addProduct/{name}, {category}, {brand}")]
        public ActionResult addProduct(string name, string category, string brand)
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
            _db.Products.Add(new Product { ProductName = name, Category = category, Brand = brand });
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

            _db.Remove(obj);
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

            _db.Remove(obj);
            _db.SaveChanges();
            return Ok("Product successfully deleted");
        }
    }
}
