using Microsoft.AspNetCore.Mvc;
using CatalogService.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Security.Authentication;

namespace CatalogService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : Controller
    {
        private readonly AppDbContext _db;
        private readonly HttpClient _client;
        private readonly string _uri;
        public ReservationController(AppDbContext db)
        {
            _db = db;
            _uri = "https://localhost:7213";
            var handler = new HttpClientHandler()
            {
                SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls
            };
            _client = new HttpClient(handler);
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpGet("GetReservedProducts")]
        public ActionResult GetReservedProducts()
        {
            var products = _db.ReservedProducts;
            if (products == null)
                return NotFound("Products not found");
            return Json(products);
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpGet("GetReservedProductsByOrderId/{orderId}")]
        public ActionResult GetReservedProductsByOrderId(Guid orderId)
        {
            var products = _db.ReservedProducts.Where(c => c.OrderId == orderId);
            if (products == null)
                return NotFound("Products not found");
            return Json(products);
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [HttpPost("ProductReservation/{orderId}")]
        public async Task<ActionResult> ProductReservation(Guid orderId)
        {
            var httpResponse = await _client.GetFromJsonAsync<IList<ReservedProduct>>(_uri + $"/api/Order/GetProductsInOrder/{orderId}");

            if (httpResponse == null)
                return BadRequest("Order not found");

            foreach (var item in httpResponse)
            {
                var productInDataBase =  _db.Products.FirstOrDefault(c => c.Id == item.ProductId);
                if (productInDataBase == null)
                    return NotFound($"Product with id:{item.ProductId} not found");
                if (productInDataBase.Count - item.Count < 0)
                    return BadRequest("Invalid count");
                productInDataBase.Count -= item.Count;
                Console.WriteLine(productInDataBase.Count);
                _db.Products.Update(productInDataBase);
                _db.ReservedProducts.Add(new ReservedProduct { OrderId = item.OrderId, ProductId = item.ProductId, Count = item.Count});
            }


            _db.SaveChanges();

            return Ok();
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [HttpDelete("ReservationCancel/{orderId}")]
        public ActionResult ReservationCancel(Guid orderId)
        {
            var products = _db.ReservedProducts.Where(c => c.OrderId == orderId);
            if (products == null)
                return BadRequest("Invalid order id");

            foreach(var product in products)
            {
                var productInDatatBase = _db.Products.FirstOrDefault(c => c.Id == product.ProductId);
                if (productInDatatBase == null)
                    return NotFound($"Object with id:{product.ProductId} not found in Product table");
                productInDatatBase.Count += product.Count;
                _db.Products.Update(productInDatatBase);
                _db.ReservedProducts.Remove(product);
            }
                

            _db.SaveChanges();

            return Ok("Order cancelled");
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [HttpDelete("CompleteOrder/{orderId}")]
        public async Task<ActionResult> CompleteOrder(Guid orderId)
        {
            var products = _db.ReservedProducts.Where(_ => _.OrderId == orderId);
            if (products == null)
                return NotFound($"Order with id:{orderId} not found");

            var httpResponse = await _client.DeleteAsync(_uri + $"/api/Order/DeleteOrder/{orderId}");
            if (httpResponse == null)
                return BadRequest("Unable get response from OrderService");
            if (httpResponse.StatusCode == HttpStatusCode.NotFound)
                return NotFound("Order not found");

            _db.ReservedProducts.RemoveRange(products);
            _db.SaveChanges();

            return Ok("Order completed");

        }
    }
}
