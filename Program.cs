using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(opt =>
   opt.UseInMemoryDatabase("CavalosDB"));

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGet("/", () => "Bom Dia!");

app.MapGet("/cavalos", async (AppDbContext db) => await db.Cavalos.ToListAsync());

app.MapPost("/cavalos", async (Cavalo cavalo, AppDbContext db) =>
{
    db.Cavalos.Add(cavalo);
    await db.SaveChangesAsync();
    return Results.Created($"/tarefas/{cavalo.Id}", cavalo);
});

app.MapPut("/cavalo/{id}", async (int id, Cavalo inputCavalo, AppDbContext db) =>
{
    var cavalo = await db.Cavalos.FirstAsync(c => c.Id == id);
    if (cavalo is null) return Results.NotFound();

    cavalo.Nome = inputCavalo.Nome;
    cavalo.Raca = inputCavalo.Raca;

    await db.SaveChangesAsync();
    return Results.Ok(inputCavalo);
});

app.MapDelete("/cavalo/{id}", async (int id, AppDbContext db) =>
{
    if(await db.Cavalos.FindAsync(id) is Cavalo cavalo)
    {
        db.Cavalos.Remove(cavalo);
        await db.SaveChangesAsync();
        return Results.Ok(cavalo);
    } 
    return Results.NotFound();
});

app.Run("http://localhost:3000");


class Cavalo
{
    public int Id { get; set; }
    public string? Nome { get; set; }
    public string? Raca { get; set; }
}

class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }

    public DbSet<Cavalo> Cavalos => Set<Cavalo>();
}