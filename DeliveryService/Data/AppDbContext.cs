using Microsoft.EntityFrameworkCore;
using DeliveryService.Models;

namespace DeliveryService
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<DeliveryOrder> DeliveryOrders { get; set; }
    }
}
