using Pizzeria_Rest_Api.Classi;

var builder = WebApplication.CreateBuilder(args);

// Configuro la policy CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()   // permette qualsiasi origine
            .AllowAnyMethod()   // permette GET, POST, PUT, DELETE, ecc.
            .AllowAnyHeader();  // permette tutti gli headers
    });
});

var app = builder.Build();

app.UseHttpsRedirection();

var menu = new List<Pizza>
{
    new Pizza
    {
        Id = 1,
        Nome = "Margherita",
        IsVegana = false,
        Prezzo = 6.50m,
        Ingredientes = new List<Ingrediente>
        {
            new Ingrediente { Nome = "Pomodoro", IsAllergene = false },
            new Ingrediente { Nome = "Mozzarella", IsAllergene = true },
            new Ingrediente { Nome = "Basilico", IsAllergene = false }
        }
    },
    new Pizza
    {
        Id = 2,
        Nome = "Vegana",
        IsVegana = true,
        Prezzo = 7.00m,
        Ingredientes = new List<Ingrediente>
        {
            new Ingrediente { Nome = "Pomodoro", IsAllergene = false },
            new Ingrediente { Nome = "Formaggio vegano", IsAllergene = false },
            new Ingrediente { Nome = "Zucchine", IsAllergene = false },
            new Ingrediente { Nome = "Peperoni", IsAllergene = false }
        }
    },
    new Pizza
    {
        Id = 3,
        Nome = "Quattro Formaggi",
        IsVegana = false,
        Prezzo = 8.00m,
        Ingredientes = new List<Ingrediente>
        {
            new Ingrediente { Nome = "Mozzarella", IsAllergene = true },
            new Ingrediente { Nome = "Gorgonzola", IsAllergene = true },
            new Ingrediente { Nome = "Fontina", IsAllergene = true },
            new Ingrediente { Nome = "Parmigiano", IsAllergene = true }
        }
    },
    new Pizza
    {
        Id = 4,
        Nome = "Diavola",
        IsVegana = false,
        Prezzo = 7.50m,
        Ingredientes = new List<Ingrediente>
        {
            new Ingrediente { Nome = "Pomodoro", IsAllergene = false },
            new Ingrediente { Nome = "Mozzarella", IsAllergene = true },
            new Ingrediente { Nome = "Salame piccante", IsAllergene = false }
        }
    }
};
app.UseCors("AllowAll");
app.MapGet("/api/menu", () =>
{
    //var pizzaDTO = menu.Select(p => new
    //{
    //    Nom = p.Nome,
    //    Veg = p.IsVegana,
    //    Pre = p.Prezzo,
    //    Ing = p.Ingredientes.Select(i => new
    //    {
    //        Nom = i.Nome,
    //        All = i.IsAllergene
    //    })
    //});

    //return Results.Ok(pizzaDTO);

    return Results.Ok(menu);
});

app.MapPost("/api/menu", (Pizza piz) =>
{
    piz.Id = menu.Count + 1;
    menu.Add(piz);

    return Results.Created("/api/menu/" + piz.Nome, null);
});

app.MapDelete("/api/menu/{id}", (int id) =>
{
    var pizzaDaRimuovere = menu.FirstOrDefault(p => p.Id == id);
    if (pizzaDaRimuovere is null)
        return Results.NotFound();

    menu.Remove(pizzaDaRimuovere);
    return Results.Ok();
});



app.Run();