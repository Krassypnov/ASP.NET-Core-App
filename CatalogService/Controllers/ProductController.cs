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
    }
}
