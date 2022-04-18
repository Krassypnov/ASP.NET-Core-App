using CatalogService;
using CatalogService.Models;
using Microsoft.EntityFrameworkCore;


List<Person> users = new List<Person>
{
    new() { Id = Guid.NewGuid().ToString(), Name = "Tom", Age = 37 },
    new() { Id = Guid.NewGuid().ToString(), Name = "Bob", Age = 41 },
    new() { Id = Guid.NewGuid().ToString(), Name = "Sam", Age = 24 }
};

var builder = WebApplication.CreateBuilder(args);
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


app.MapGet("/getdatetime", () =>
{
    var someData = "Time: ";

    return someData + DateTime.Now.ToString();
}).WithName("GetTime");

app.MapGet("/api/users", () => users).WithName("GetUsers");

app.MapGet("/api/users/{id}", (string id) =>
{
    Person? person = users.FirstOrDefault(x => x.Id == id);
    if (person == null) return Results.NotFound(new { message = "User not found" });

    return Results.Json(person);
});

app.MapPost("/api/users/{id}", (Person person) =>
{
    
    if (users.Contains(person))
    {
        return Results.NotFound(new { message = "User already exist" });
    }

    users.Add(person);
    return Results.Ok(new { message = "Person was added" });

});


app.MapDelete("/api/users/{id}", (string id) =>
{
    // получаем пользователя по id
    Person? user = users.FirstOrDefault(u => u.Id == id);

    // если не найден, отправляем статусный код и сообщение об ошибке
    if (user == null) return Results.NotFound(new { message = "Пользователь не найден" });

    // если пользователь найден, удаляем его
    users.Remove(user);
    return Results.Json(user);
});


app.Run();

