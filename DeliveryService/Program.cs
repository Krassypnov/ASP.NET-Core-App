using System.Net.Http;

namespace DeliveryService
{
    public class Program 
    {
        private static readonly HttpClient client = new HttpClient();
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


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


            
            app.MapGet("/api/users", async (string Name, int Age) =>
            {
                var userId = Guid.NewGuid().ToString();
                //var responsePerson = await client.GetFromJsonAsync<Person>($"https://localhost:7113/api/users/{userId}");
                var responsePerson = await client.PostAsJsonAsync<Person>($"https://localhost:7113/api/users/{userId}", new Person { Id = userId, Age = Age, Name = Name});
                Console.WriteLine(responsePerson);
                return responsePerson;
            });


            app.Run();
        }
    }

}

