using Microsoft.EntityFrameworkCore;


namespace DeliveryService
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        
    }
}
