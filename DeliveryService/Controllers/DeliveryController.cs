using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Security.Authentication;
using DeliveryService.Data;
using DeliveryService.Models;

namespace DeliveryService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryController : Controller
    {
        private readonly AppDbContext _db;
        private readonly HttpClient _client;
        private readonly string _uriOrder;
        private readonly string _uriCatalog;

        public DeliveryController(AppDbContext db)
        {
            _db = db;
            _uriOrder = "https://localhost:7213";
            _uriCatalog = "https://localhost:7113";

            var handler = new HttpClientHandler()
            {
                SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls
            };
            _client = new HttpClient(handler);
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [HttpGet("GetOrderInfo/{orderId}")]
        public async Task<ActionResult> GetOrderInfo(Guid orderId)
        {
            var order = _db.DeliveryOrders.FirstOrDefault(c => c.OrderId == orderId);
            if (order == null)
                return NotFound($"Order with id:{orderId} not found");
            var httpResponse = await _client.GetFromJsonAsync <OrderClass>(_uriOrder + $"/api/Order/GetOrderById/{orderId}");
            if (httpResponse == null)
                return BadRequest("Unable get response from OrderService");
            

            return Json(httpResponse);
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [HttpGet("GetOrders")]
        public async Task<ActionResult> GetOrders()
        {
            var orders = _db.DeliveryOrders;
            if (orders == null)
                return NotFound($"Orders not found");

            List<OrderClass> ordersList = new List<OrderClass>();
            foreach (var order in orders)
            {
                var httpResponse = await _client.GetFromJsonAsync<OrderClass>(_uriOrder + $"/api/Order/GetOrderById/{order.OrderId}");
                if (httpResponse == null)
                    return BadRequest("Unable get response from OrderService");
                ordersList.Add(httpResponse);
            }

            return Json(ordersList);
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [HttpGet("GetProductsFromOrder/{orderId}")]
        public async Task<ActionResult> GetProductsFromOrder(Guid orderId)
        {
            var order = _db.DeliveryOrders.FirstOrDefault(c => c.OrderId == orderId);
            if (order == null)
                return NotFound($"Order with id:{orderId} not found");
            var httpResponse = await _client.GetFromJsonAsync<IList<ProductClass>>(_uriCatalog + $"/api/Reservation/GetReservedProductsByOrderId/{orderId}");
            if (httpResponse == null)
                return BadRequest("Unable get response from CatalogService");

            return Json(httpResponse);
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [HttpPost("AddDeliveryOrder/{orderId}")]
        public ActionResult AddDeliveryOrder(Guid orderId)
        {
            _db.DeliveryOrders.Add(new DeliveryOrder { OrderId = orderId });
            _db.SaveChanges();
            return Ok("Order added");
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpDelete("DeliverOrder/{orderId}")]
        public async Task<ActionResult> DeliverOrder(Guid orderId)
        {
            var order = _db.DeliveryOrders.FirstOrDefault(c => c.OrderId == orderId);
            if (order == null)
                return NotFound($"Order with id:{orderId} not found");
            var httpResponse = await _client.DeleteAsync(_uriCatalog + $"/api/Reservation/CompleteOrder/{orderId}");
            if (httpResponse == null)
                return BadRequest("Unable get response from CatalogService");

            _db.DeliveryOrders.Remove(order);
            _db.SaveChanges();

            return Ok("Order delivered");
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpDelete("ReturnOrder/{orderId}")]
        public async Task<ActionResult> ReturnOrder(Guid orderId)
        {
            var order = _db.DeliveryOrders.FirstOrDefault(c => c.OrderId == orderId);
            if (order == null)
                return NotFound($"Order with id:{orderId} not found");
            var httpResponse = await _client.DeleteAsync(_uriCatalog + $"/api/Reservation/ReservationCancel/{orderId}");
            if (httpResponse == null)
                return BadRequest("Unable get response from CatalogService");

            _db.DeliveryOrders.Remove(order);
            _db.SaveChanges();

            return Ok("Order returned");
        }
    }
}
