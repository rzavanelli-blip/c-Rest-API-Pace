using Esame_c__REST_API_Autore_libro.Modelli;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuro la policy CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseSwagger();
app.UseSwaggerUI();

// Middleware autenticazione
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/api/login")
    {
        await next.Invoke();
        return;
    }

    if (context.Request.Headers.TryGetValue("Access", out var valoreToken) && valoreToken == "AB12345")
    {
        await next.Invoke();
    }
    else
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        await context.Response.WriteAsync("Accesso negato: token mancante o non valido.");
    }
});

// ----------------- LOGIN -----------------
app.MapPost("/api/login", (Credenziali cred) =>
{
    if (string.IsNullOrWhiteSpace(cred.Password) || string.IsNullOrWhiteSpace(cred.Username))
        return Results.BadRequest("Username e/o password mancanti.");

    if (cred.Username == "Cristian" && cred.Password == "Cristian")
        return Results.Ok(new { token = "AB12345" });

    return Results.BadRequest("Credenziali non valide.");
})
.WithName("Login")
.WithSummary("Login utente")
.WithDescription("Esegue il login e restituisce un token statico di esempio.")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest);

// ----------------- DATABASE FINTI -----------------
var autori = new List<Autore>
{
    new Autore { AutoreId = 1, Nome = "J.R.R. Tolkien" },
    new Autore { AutoreId = 2, Nome = "George Orwell" }
};

var libri = new List<Libro>
{
    new Libro { LibroId = 1, Titolo = "Lo Hobbit", AnnoPubblicazione = 1937, AutoreId = 1 },
    new Libro { LibroId = 2, Titolo = "1984", AnnoPubblicazione = 1949, AutoreId = 2 },
    new Libro { LibroId = 3, Titolo = "La Fattoria degli Animali", AnnoPubblicazione = 1945, AutoreId = 2 },
    new Libro { LibroId = 4, Titolo = "Il Signore degli Anelli", AnnoPubblicazione = 1954, AutoreId = 1 }
};

autori.First(a => a.AutoreId == 1).Libri.AddRange(libri.Where(l => l.AutoreId == 1));
autori.First(a => a.AutoreId == 2).Libri.AddRange(libri.Where(l => l.AutoreId == 2));

// ----------------- ENDPOINT AUTORI -----------------
app.MapGet("/api/autori", () => Results.Ok(autori))
    .WithName("GetAutori")
    .WithSummary("Ottieni tutti gli autori")
    .WithDescription("Restituisce la lista completa degli autori disponibili.")
    .Produces<List<Autore>>(StatusCodes.Status200OK);

app.MapGet("/api/autori/{id}", (int id) =>
{
    var autore = autori.FirstOrDefault(a => a.AutoreId == id);
    return autore is null ? Results.NotFound("Autore non trovato.") : Results.Ok(autore);
})
.WithName("GetAutoreById")
.WithSummary("Ottieni un autore per ID")
.WithDescription("Restituisce l’autore con l’ID specificato.")
.Produces<Autore>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

app.MapPost("/api/autori", (Autore nuovoAutore) =>
{
    nuovoAutore.AutoreId = autori.Any() ? autori.Max(a => a.AutoreId) + 1 : 1;
    autori.Add(nuovoAutore);
    return Results.Created($"/api/autori/{nuovoAutore.AutoreId}", nuovoAutore);
})
.WithName("CreateAutore")
.WithSummary("Crea un nuovo autore")
.WithDescription("Aggiunge un autore al database in memoria.")
.Produces<Autore>(StatusCodes.Status201Created);

app.MapPut("/api/autori/{id}", (int id, Autore autoreAggiornato) =>
{
    var autoreEsistente = autori.FirstOrDefault(a => a.AutoreId == id);
    if (autoreEsistente is null) return Results.NotFound("Autore non trovato.");
    autoreEsistente.Nome = autoreAggiornato.Nome;
    return Results.NoContent();
})
.WithName("UpdateAutore")
.WithSummary("Aggiorna un autore")
.WithDescription("Aggiorna i dati di un autore esistente.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound);

app.MapDelete("/api/autori/{id}", (int id) =>
{
    var autoreDaRimuovere = autori.FirstOrDefault(a => a.AutoreId == id);
    if (autoreDaRimuovere is null) return Results.NotFound("Autore non trovato.");
    libri.RemoveAll(l => l.AutoreId == id);
    autori.Remove(autoreDaRimuovere);
    return Results.Ok("Autore e libri correlati eliminati.");
})
.WithName("DeleteAutore")
.WithSummary("Elimina un autore")
.WithDescription("Elimina un autore e tutti i suoi libri associati.")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

// ----------------- ENDPOINT LIBRI -----------------
app.MapGet("/api/libri", () => Results.Ok(libri))
    .WithName("GetLibri")
    .WithSummary("Ottieni tutti i libri")
    .WithDescription("Restituisce la lista completa dei libri disponibili.")
    .Produces<List<Libro>>(StatusCodes.Status200OK);

app.MapGet("/api/libri/{id}", (int id) =>
{
    var libro = libri.FirstOrDefault(l => l.LibroId == id);
    return libro is null ? Results.NotFound("Libro non trovato.") : Results.Ok(libro);
})
.WithName("GetLibroById")
.WithSummary("Ottieni un libro per ID")
.WithDescription("Restituisce il libro con l’ID specificato.")
.Produces<Libro>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

app.MapPost("/api/libri", (Libro nuovoLibro) =>
{
    var autoreEsistente = autori.FirstOrDefault(a => a.AutoreId == nuovoLibro.AutoreId);
    if (autoreEsistente is null) return Results.BadRequest("Autore non valido.");

    nuovoLibro.LibroId = libri.Any() ? libri.Max(l => l.LibroId) + 1 : 1;
    libri.Add(nuovoLibro);
    autoreEsistente.Libri.Add(nuovoLibro);
    return Results.Created($"/api/libri/{nuovoLibro.LibroId}", nuovoLibro);
})
.WithName("CreateLibro")
.WithSummary("Crea un nuovo libro")
.WithDescription("Aggiunge un nuovo libro e lo associa a un autore esistente.")
.Produces<Libro>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest);

app.MapPut("/api/libri/{id}", (int id, Libro libroAggiornato) =>
{
    var libroEsistente = libri.FirstOrDefault(l => l.LibroId == id);
    if (libroEsistente is null) return Results.NotFound("Libro non trovato.");
    libroEsistente.Titolo = libroAggiornato.Titolo;
    libroEsistente.AnnoPubblicazione = libroAggiornato.AnnoPubblicazione;
    libroEsistente.AutoreId = libroAggiornato.AutoreId;
    return Results.NoContent();
})
.WithName("UpdateLibro")
.WithSummary("Aggiorna un libro")
.WithDescription("Aggiorna i dati di un libro esistente.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound);

app.MapDelete("/api/libri/{id}", (int id) =>
{
    var libroDaRimuovere = libri.FirstOrDefault(l => l.LibroId == id);
    if (libroDaRimuovere is null) return Results.NotFound("Libro non trovato.");
    libri.Remove(libroDaRimuovere);
    var autoreCorrelato = autori.FirstOrDefault(a => a.AutoreId == libroDaRimuovere.AutoreId);
    autoreCorrelato?.Libri.Remove(libroDaRimuovere);
    return Results.Ok("Libro eliminato con successo.");
})
.WithName("DeleteLibro")
.WithSummary("Elimina un libro")
.WithDescription("Elimina un libro e lo rimuove anche dalla lista dell’autore.")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

// ----------------- ENDPOINT EXTRA -----------------
app.MapGet("/api/autori/{autoreId}/libri", (int autoreId) =>
{
    var autore = autori.FirstOrDefault(a => a.AutoreId == autoreId);
    if (autore is null) return Results.NotFound("Autore non trovato.");
    return Results.Ok(autore.Libri);
})
.WithName("GetLibriByAutore")
.WithSummary("Ottieni i libri di un autore")
.WithDescription("Restituisce tutti i libri scritti da un determinato autore.")
.Produces<List<Libro>>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

app.Run();
