using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using OrderService.Models;
using System.Net;
using System.Security.Authentication;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly AppDbContext _db;
        private readonly HttpClient _client;
        public OrderController(AppDbContext db)
        {
            _db = db;
            var handler = new HttpClientHandler()
            {
                SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls
            };
            _client = new HttpClient(handler);
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpGet]
        public async Task<IList<Product>?> getProducts()
        {
            var httpResponse = await _client.GetFromJsonAsync<IList<Product>>("https://localhost:7113/api/Product");

            if (httpResponse == null)
                return null;

            return httpResponse;
        }

       
    }
}
