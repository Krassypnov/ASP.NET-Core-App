using CatalogService;
using CatalogService.Models;

using Microsoft.EntityFrameworkCore;
using Swashbuckle.Swagger;

namespace CatalogService
{
    public class Program
    {
        
        public static void Main(string[] args)
        {
           
            List<Person> users = new List<Person>
            {
                new() { Id = Guid.NewGuid().ToString(), Name = "Tom", Age = 37 },
                new() { Id = Guid.NewGuid().ToString(), Name = "Bob", Age = 41 },
                new() { Id = Guid.NewGuid().ToString(), Name = "Sam", Age = 24 }
            };

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(
                builder.Configuration.GetConnectionString("CatalogConnection")
                ));

            // Add services to the container.
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Category}/{action=getCategories}/{id?}");
            });



            app.Run();

        }
    }
    
}





