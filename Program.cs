using Frameworks1.DTOs;
using Frameworks1.Errors;
using Frameworks1.Middleware;
using Frameworks1.Repositories;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IPlayerRepository, PlayerRepository>();


var app = builder.Build();

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<TimingAndLogMiddleware>();



app.MapGet("/api/items", (IPlayerRepository repo) =>
{
    return Results.Ok(repo.GetAll());
});

app.MapGet("/api/items/{id:guid}", (Guid id, IPlayerRepository repo) =>
{
    var item = repo.GetById(id);
    if (item is null)
        throw new NotFoundException("Ёлемент не найден");

    return Results.Ok(item);
});

app.MapPost("/api/items", (HttpContext ctx, CreatePlayerRequest request, IPlayerRepository repo) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
        throw new ValidationException("ѕоле name не должно быть пустым");

    if (request.Score < 0)
        throw new ValidationException("ѕоле score не может быть отрицательным");

    var created = repo.Create(request.Name.Trim(), request.Score);

    var location = $"/api/items/{created.Id}";
    ctx.Response.Headers.Location = location;

    return Results.Created(location, created);
});

app.MapGet("/", () => "Hello World!");

app.Run();
