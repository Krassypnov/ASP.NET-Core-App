using Microsoft.AspNetCore.Mvc;
using CatalogService.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace CatalogService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : Controller
    {
        private readonly AppDbContext _db;

        public BrandController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public ActionResult getBrands()
        {
            IEnumerable<Brand> obj = _db.Brands;
            return Json(obj);
        }

        [HttpGet("{id}")]
        public ActionResult getBrand(Guid id)
        {
            var obj = _db.Brands.FirstOrDefault(c => c.Id == id);
            if (obj == null)
                return Json(null);
            return Json(obj);
        }


        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [HttpPost("{name}")]
        public ActionResult addBrand(string name)
        {
            var obj = _db.Brands.FirstOrDefault(c => c.BrandName == name);
            if (obj != null)
            {
                return BadRequest("Brand already exists");
            }

            _db.Brands.Add(new Brand { BrandName = name });
            _db.SaveChanges();

            return StatusCode((int)HttpStatusCode.OK);
        }


        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpDelete("deleteBrandByName/{name}")]
        public ActionResult deleteBrandByName(string name)
        {
            var obj = _db.Brands.FirstOrDefault(c => c.BrandName == name);
            if (obj == null)
                return NotFound();

            _db.Brands.Remove(obj);
            _db.SaveChanges();
            return Ok("Product successfully deleted");
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpDelete("deleteBrandById/{name}")]
        public ActionResult deleteBrandById(Guid id)
        {
            var obj = _db.Brands.FirstOrDefault(c => c.Id == id);
            if (obj == null)
                return NotFound();

            _db.Brands.Remove(obj);
            _db.SaveChanges();
            return Ok("Product successfully deleted");
        }
    }
}
