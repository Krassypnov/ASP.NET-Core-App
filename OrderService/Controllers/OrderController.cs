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
        public async Task<IList<Product>?> getProducts()
        {
            var httpResponse = await _client.GetFromJsonAsync<IList<Product>>(_uri + "/api/Product");

            if (httpResponse == null)
                return null;

            return httpResponse;
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpGet("getProductById/{id}")]
        public async Task<Product?> getProductById(Guid id)
        {
            var httpResponse = await _client.GetFromJsonAsync<Product>(_uri + $"/api/Product/getProductById/{id}");

            if (httpResponse == null)
                return null;

            return httpResponse;
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpGet("getProductByName/{name}")]
        public async Task<Product?> GetProductByName(string name)
        {
            var httpResponse = await _client.GetFromJsonAsync<Product>(_uri + $"/api/Product/getProductByName/{name}");

            if (httpResponse == null)
                return null;

            return httpResponse;
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpGet("getProductsByCategory/{name}")]
        public async Task<IList<Product>?> GetProductsByCategory(string name)
        {
            var httpResponse = await _client.GetFromJsonAsync<IList<Product>?>(_uri + $"/api/Product/getProductsByCategory/{name}");

            if (httpResponse == null)
                return null;

            return httpResponse;
        }

        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpGet("getProductsByBrand/{name}")]
        public async Task<IList<Product>?> GetProductsByBrand(string name)
        {
            var httpResponse = await _client.GetFromJsonAsync<IList<Product>?>(_uri + $"/api/Product/getProductsByBrand/{name}");

            if (httpResponse == null)
                return null;

            return httpResponse;
        }

    }
}
