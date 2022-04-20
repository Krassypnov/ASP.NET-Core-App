using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using OrderService.Models;
using System.Net;
using System.Security.Authentication;
using OrderService.Data;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly AppDbContext _db;
        private readonly HttpClient _client;
        private readonly string _uri;
        public OrderController(AppDbContext db)
        {
            _db = db;
            var handler = new HttpClientHandler()
            {
                SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls
            };
            _client = new HttpClient(handler);
            _uri = "https://localhost:7113";
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpGet("getProducts")]
        public async Task<IList<ProductClass>?> getProducts()
        {
            var httpResponse = await _client.GetFromJsonAsync<IList<ProductClass>>(_uri + "/api/Product");

            if (httpResponse == null)
                return null;

            return httpResponse;
        }

        

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpGet("getProductById/{id}")]
        public async Task<ProductClass?> getProductById(Guid id)
        {
            var httpResponse = await _client.GetFromJsonAsync<ProductClass>(_uri + $"/api/Product/getProductById/{id}");

            if (httpResponse == null)
                return null;

            return httpResponse;
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpGet("getProductByName/{name}")]
        public async Task<ProductClass?> GetProductByName(string name)
        {
            var httpResponse = await _client.GetFromJsonAsync<ProductClass>(_uri + $"/api/Product/getProductByName/{name}");

            if (httpResponse == null)
                return null;

            return httpResponse;
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpGet("getProductsByCategory/{name}")]
        public async Task<IList<ProductClass>?> GetProductsByCategory(string name)
        {
            var httpResponse = await _client.GetFromJsonAsync<IList<ProductClass>?>(_uri + $"/api/Product/getProductsByCategory/{name}");

            if (httpResponse == null)
                return null;

            return httpResponse;
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpGet("getProductsByBrand/{name}")]
        public async Task<IList<ProductClass>?> GetProductsByBrand(string name)
        {
            var httpResponse = await _client.GetFromJsonAsync<IList<ProductClass>?>(_uri + $"/api/Product/getProductsByBrand/{name}");

            if (httpResponse == null)
                return null;

            return httpResponse;
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpPost("makeOrder")]
        public ActionResult makeOrder()
        {
            Guid newOrderId = Guid.NewGuid();

            _db.Orders.Add(new Order { Id = newOrderId, ClientName = "", ClientAddress = "", PhoneNumber = "", CreatedDate = DateTime.Now});
            _db.SaveChanges();
            return Json(newOrderId);
        }
        

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpPost("addProductToOrder/{orderId}, {productId}, {count}")]
        public ActionResult addProductToOrder(Guid orderId, Guid productId, int count)
        {
            var order = _db.Orders.FirstOrDefault(c => c.Id == orderId);
            

            if (order == null)
                return BadRequest("Order does not exists");

            if (count < 1)
                return BadRequest("Incorrect count value");

            var newProduct = _db.Products.FirstOrDefault(c => c.Id == productId);

            if (newProduct == null)
            {
                _db.Products.Add(new Product { Id = productId, OrderId = orderId, Count = count });
            }
            else
            {
                newProduct.Count += count;
                _db.Products.Update(newProduct);
            }

            _db.SaveChanges();
            return Ok("Product successfully added");
        }


    }
}
