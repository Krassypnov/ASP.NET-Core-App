using Microsoft.AspNetCore.Mvc;
using CatalogService.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace CatalogService.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _db;

        public CategoryController(AppDbContext db)
        {
            _db = db;
        }
        
        [HttpGet]
        public ActionResult getCategories()
        {
            IEnumerable<Category> obj = _db.Categories;
            return Json(obj);
        }

        [HttpGet("{id}")]
        public ActionResult getCategory(Guid id)
        {
            var obj = _db.Categories.FirstOrDefault(c => c.Id == id);
            if (obj == null)
                return Json(null);
            return Json(obj);
        }

        
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [HttpPost("{name}")]
        public ActionResult addCategory(string name)
        {
            var obj = _db.Categories.FirstOrDefault(c => c.CategoryName == name);
            if (obj != null)
            {
                return BadRequest("Category already exists");
            }

            _db.Categories.Add(new Category { CategoryName = name });
            _db.SaveChanges();

            return StatusCode((int)HttpStatusCode.OK);
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpDelete("deleteCategoryByName/{name}")]
        public ActionResult deleteCategoryByName(string name)
        {
            var obj = _db.Categories.FirstOrDefault(c => c.CategoryName == name);
            if (obj == null)
                return NotFound();

            _db.Categories.Remove(obj);
            _db.SaveChanges();
            return Ok("Product successfully deleted");
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpDelete("deleteCategoryById/{name}")]
        public ActionResult deleteCategoryById(Guid id)
        {
            var obj = _db.Categories.FirstOrDefault(c => c.Id == id);
            if (obj == null)
                return NotFound();

            _db.Categories.Remove(obj);
            _db.SaveChanges();
            return Ok("Product successfully deleted");
        }
    }
}
