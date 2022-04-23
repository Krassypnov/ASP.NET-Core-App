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
        [HttpGet("GetProducts")]
        public async Task<IList<ProductClass>?> GetProducts()
        {
            var httpResponse = await _client.GetFromJsonAsync<IList<ProductClass>>(_uri + "/api/Product");

            if (httpResponse == null)
                return null;

            return httpResponse;
        }

        

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpGet("GetProductById/{id}")]
        public async Task<ProductClass?> GetProductById(Guid id)
        {
            var httpResponse = await _client.GetFromJsonAsync<ProductClass>(_uri + $"/api/Product/getProductById/{id}");

            if (httpResponse == null)
                return null;

            return httpResponse;
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpGet("GetProductByName/{name}")]
        public async Task<ProductClass?> GetProductByName(string name)
        {
            var httpResponse = await _client.GetFromJsonAsync<ProductClass>(_uri + $"/api/Product/getProductByName/{name}");

            if (httpResponse == null)
                return null;

            return httpResponse;
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpGet("GetProductsByCategory/{name}")]
        public async Task<IList<ProductClass>?> GetProductsByCategory(string name)
        {
            var httpResponse = await _client.GetFromJsonAsync<IList<ProductClass>?>(_uri + $"/api/Product/getProductsByCategory/{name}");

            if (httpResponse == null)
                return null;

            return httpResponse;
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpGet("GetProductsByBrand/{name}")]
        public async Task<IList<ProductClass>?> GetProductsByBrand(string name)
        {
            var httpResponse = await _client.GetFromJsonAsync<IList<ProductClass>?>(_uri + $"/api/Product/getProductsByBrand/{name}");

            if (httpResponse == null)
                return null;

            return httpResponse;
        }


        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpGet("GetProductsInOrder/{orderId}")]
        public ActionResult GetProductsInOrder(Guid orderId)
        {
            var products = _db.Products.Where(x => x.OrderId == orderId);

            if (products == null)
                return NotFound("Prdoucts not found");

            return Json(products);
        }


        [SwaggerResponse((int)HttpStatusCode.OK)]
        [HttpPost("MakeOrder")]
        public ActionResult MakeOrder()
        {
            Guid newOrderId = Guid.NewGuid();

            _db.Orders.Add(new Order { Id = newOrderId, ClientName = "",
                                       ClientAddress = "", PhoneNumber = "", 
                                       CreatedDate = DateTime.Now, IsConfirmedOrder = false, 
                                       IsDelivery = false});
            _db.SaveChanges();
            return Json(newOrderId);
        }
        

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [HttpPost("AddProductToOrder/{orderId}, {productId}, {count}")]
        public ActionResult AddProductToOrder(Guid orderId, Guid productId, int count)
        {
            var order = _db.Orders.FirstOrDefault(c => c.Id == orderId);
            

            if (order == null)
                return BadRequest("Order does not exists");

            if (count < 1)
                return BadRequest("Incorrect count value");

            var newProduct = _db.Products.FirstOrDefault(c => c.ProductId == productId && c.OrderId == orderId);

            if (newProduct == null)
            {
                _db.Products.Add(new Product { ProductId = productId, OrderId = orderId, Count = count });
            }
            else
            {
                newProduct.Count += count;
                _db.Products.Update(newProduct);
            }

            _db.SaveChanges();
            return Ok("Product successfully added");
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [HttpPost("ConfirmOrder/{orderId},{clientName},{clientAddress},{phoneNumber},{isDelivery}")]
        public async Task<ActionResult> ConfirmOrder(Guid orderId, string clientName, string clientAddress, string phoneNumber, bool isDelivery)
        {
            if (clientName.Length == 0 || clientAddress.Length == 0 || phoneNumber.Length == 0)
                return BadRequest("Invalid parameters");
            

            var order = _db.Orders.FirstOrDefault(c => c.Id == orderId);

            if (order == null)
                return NotFound("Order not found");

            order.ClientName = clientName; 
            order.ClientAddress = clientAddress;
            order.PhoneNumber = phoneNumber;
            order.IsDelivery = isDelivery;
            order.IsConfirmedOrder = true;

            _db.Orders.Update(order);
            _db.SaveChanges();

            if (order.IsDelivery)
            {
                // Make delivery
            }
            else
            {
                var httpResponse = await _client.GetAsync(_uri + $"/api/Reservation/productReservation/{orderId}");
                if (httpResponse == null)
                    return BadRequest("Unable to get response from CatalogService");
                if (httpResponse.StatusCode == HttpStatusCode.BadRequest)
                    return BadRequest("Invalid count");
                return Ok(httpResponse);
            }


            return Ok("Order successfully confirmed");
        }

    }
}
