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
        public ActionResult GetBrands()
        {
            IEnumerable<Brand> obj = _db.Brands;
            return Json(obj);
        }

        [HttpGet("{id}")]
        public ActionResult GetBrand(Guid id)
        {
            var obj = _db.Brands.FirstOrDefault(c => c.Id == id);
            if (obj == null)
                return Json(null);
            return Json(obj);
        }


        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [HttpPost("{name}")]
        public ActionResult AddBrand(string name)
        {
            var obj = _db.Brands.FirstOrDefault(c => c.BrandName == name);
            if (obj != null)
            {
                return BadRequest("Brand already exists");
            }

            _db.Brands.Add(new Brand { BrandName = name });
            _db.SaveChanges();

            return Ok();
        }


        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpDelete("DeleteBrandByName/{name}")]
        public ActionResult DeleteBrandByName(string name)
        {
            var obj = _db.Brands.FirstOrDefault(c => c.BrandName == name);
            if (obj == null)
                return NotFound();

            _db.Brands.Remove(obj);
            _db.SaveChanges();
            return Ok("Brand successfully deleted");
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpDelete("DeleteBrandById/{id}")]
        public ActionResult DeleteBrandById(Guid id)
        {
            var obj = _db.Brands.FirstOrDefault(c => c.Id == id);
            if (obj == null)
                return NotFound();

            _db.Brands.Remove(obj);
            _db.SaveChanges();
            return Ok("Brand successfully deleted");
        }
    }
}
