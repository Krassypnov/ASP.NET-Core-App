using Microsoft.EntityFrameworkCore;
using DeliveryService.EasyNetQ;
using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using System.Reflection;

namespace DeliveryService
{
    public class Program 
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(
                builder.Configuration.GetConnectionString("DeliveryConnection")
                ));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            // Connection to bus
            var connectionString = "host=localhost";

            builder.Services.RegisterEasyNetQ(connectionString);
            builder.Services.AddSingleton<EasyNetQDispatcher>();

            //Subcribers registration
            builder.Services.AddSingleton(provider =>
                new AutoSubscriber(provider.GetRequiredService<IBus>(), "CatalogService")
                {
                    AutoSubscriberMessageDispatcher = provider.GetRequiredService<EasyNetQDispatcher>()
                });

            //Subscribers
            var interfaceType = typeof(IConsumeAsync<>);
            var assemblies = new Assembly[] { typeof(Program).Assembly };
            var types = assemblies.SelectMany(a => a.GetTypes());
            foreach (var type in types)
            {
                if (type.IsClass && !type.IsAbstract)
                {
                    if (type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType && !i.GetGenericArguments()[0].IsGenericParameter))
                    {
                        builder.Services.AddTransient(type);
                    }
                }
            }

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
                        pattern: "{controller=Delivery}/{action=getDeliveryOrders}/{id?}");
            });

            app.Services.GetRequiredService<AutoSubscriber>().Subscribe(assemblies);

            app.Run();
        }
    }

}

