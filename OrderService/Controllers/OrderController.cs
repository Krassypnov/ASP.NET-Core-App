using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using OrderService.Models;
using System.Net;
using System.Security.Authentication;
using OrderService.Data;
using EasyNetQ;
using Messages;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IPubSub _bus;
        private readonly HttpClient _client;
        private readonly string _uri;
        private readonly string _uriDelivery;
        public OrderController(AppDbContext db, IPubSub bus)
        {
            _db = db;
            _bus = bus;
            var handler = new HttpClientHandler()
            {
                SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls
            };
            _client = new HttpClient(handler);
            _uri = "https://localhost:7113";
            _uriDelivery = "https://localhost:7101";
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
            var httpResponse = await _client.GetFromJsonAsync<ProductClass>(_uri + $"/api/Product/GetProductById/{id}");

            if (httpResponse == null)
                return null;

            return httpResponse;
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpGet("GetProductByName/{name}")]
        public async Task<ProductClass?> GetProductByName(string name)
        {
            var httpResponse = await _client.GetFromJsonAsync<ProductClass>(_uri + $"/api/Product/GetProductByName/{name}");

            if (httpResponse == null)
                return null;

            return httpResponse;
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpGet("GetProductsByCategory/{name}")]
        public async Task<IList<ProductClass>?> GetProductsByCategory(string name)
        {
            var httpResponse = await _client.GetFromJsonAsync<IList<ProductClass>?>(_uri + $"/api/Product/GetProductsByCategory/{name}");

            if (httpResponse == null)
                return null;

            return httpResponse;
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpGet("GetProductsByBrand/{name}")]
        public async Task<IList<ProductClass>?> GetProductsByBrand(string name)
        {
            var httpResponse = await _client.GetFromJsonAsync<IList<ProductClass>?>(_uri + $"/api/Product/GetProductsByBrand/{name}");

            if (httpResponse == null)
                return null;

            return httpResponse;
        }


        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpGet("GetOrders")]
        public ActionResult GetOrders()
        {
            var orders = _db.Orders;

            if (orders == null)
                return NotFound("Orders not found");

            return Json(orders);
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpGet("GetOrderById/{orderId}")]
        public ActionResult GetOrderById(Guid orderId)
        {
            var order = _db.Orders.FirstOrDefault(x => x.Id == orderId);

            if (order == null)
                return NotFound("Order not found");

            return Json(order);
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpGet("GetProductsInOrder/{orderId}")]
        public ActionResult GetProductsInOrder(Guid orderId)
        {
            var products = _db.Products.Where(x => x.OrderId == orderId);

            if (products == null)
                return NotFound("Products not found");

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


        // Работа с EasyNetQ в данном методе
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

            if (order.IsConfirmedOrder)
                return BadRequest("Order already confirmed");

            var products = _db.Products.Where(c => c.OrderId == orderId);
            if (products == null)
                return BadRequest("Product list is empty");

            order.IsDelivery = isDelivery;

            if (order.IsDelivery)
            {
                var httpResponseDelivery = _client.PostAsync(_uriDelivery + $"/api/Delivery/AddDeliveryOrder/{orderId}", null);
                if (httpResponseDelivery == null)
                    return BadRequest("Unable get response from DeliveryService");
            }


            var httpResponse = await _client.GetAsync(_uri + $"/api/Reservation/InitController");
            if (httpResponse == null)
                return BadRequest("Unable to get response from CatalogService");
           
            // Отправка продуктов в CatalogService с помощью RabbitMQ
            foreach (var product in products)
                await _bus.PublishAsync(new ReservedProductMsg { OrderId = product.OrderId, ProductId = product.ProductId, Count = product.Count});


            order.ClientName = clientName;
            order.ClientAddress = clientAddress;
            order.PhoneNumber = phoneNumber;
            order.IsConfirmedOrder = true;

            _db.Orders.Update(order);
            _db.SaveChanges();

            return Ok("Order successfully confirmed");
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [HttpDelete("CompleteOrder/{orderId}")]
        public async Task<IActionResult> CompleteOrder(Guid orderId)
        {
            var order = _db.Orders.FirstOrDefault(c => c.Id == orderId);
            if (order == null)
                return NotFound($"Order with id:{orderId} not found");

            if (order.IsDelivery)
            {
                return BadRequest("Order in DeliveryService. Only DeliveryService can complete the order");
            }
            else
            {
                var httpResponse = await _client.DeleteAsync(_uri + $"/api/Reservation/CompleteOrder/{orderId}");
                if (httpResponse == null)
                    return BadRequest("Unable get response from CatalogService");
                if (httpResponse.StatusCode == HttpStatusCode.NotFound)
                    return NotFound("Order not found");

            }



            return RedirectToAction("DeleteOrder", new { orderId });
        }


        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [HttpDelete("DeleteProductFromOrder/{orderId}, {productId}")]
        public ActionResult DeleteProductFromOrder(Guid orderId, Guid productId)
        {
            var product = _db.Products.FirstOrDefault(c => c.ProductId == productId && c.OrderId == orderId);
            if (product == null)
                return NotFound($"Product with id:{orderId} not found");

            _db.Products.Remove(product);
            _db.SaveChanges();

            return RedirectToAction("DeleteOrder", new { orderId });
        }


        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [HttpDelete("CancelOrder/{orderId}")]
        public async Task<ActionResult> CancelOrder(Guid orderId)
        {
            var order = _db.Orders.FirstOrDefault(c => c.Id == orderId);
            if (order == null)
                return NotFound("Order not found");

            if (order.IsDelivery)
            {
                var httpResponse = await _client.DeleteAsync(_uriDelivery + $"/api/Delivery/ReturnOrder/{orderId}");

                if (httpResponse == null || httpResponse.StatusCode == HttpStatusCode.BadRequest)
                    return BadRequest("Unable to get response from CatalogService");
            }
            else
            {
                var httpResponse = await _client.DeleteAsync(_uri + $"/api/Reservation/ReservationCancel/{orderId}");

                if (httpResponse == null || httpResponse.StatusCode == HttpStatusCode.BadRequest)
                    return BadRequest("Unable to get response from CatalogService");
            }
            

            return RedirectToAction("DeleteOrder", new { orderId });
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [HttpDelete("DeleteOrder/{orderId}")]
        public ActionResult DeleteOrder(Guid orderId)
        {
            var order = _db.Orders.FirstOrDefault(c => c.Id == orderId);
            if (order == null)
                return NotFound("Order not found");

            var products = _db.Products.Where(c => c.OrderId == orderId);
            if (products != null)
                _db.Products.RemoveRange(products);

            _db.Orders.Remove(order);
            _db.SaveChanges();

            return Ok("Order was deleted");
        }
    }
}
