using Esame_C__REST_API.Modelli;

var builder = WebApplication.CreateBuilder(args);

var autori = new List<Autore>();
var libri = new List<Libro>();

// 
public static class MockData
{
    public static List<Autore> Autori = new()
    {
        new Autore
        {
            Id = 1,
            Nome = "Italo Calvino",
            Libri = new List<Libro>()
        },
        new Autore
        {
            Id = 2,
            Nome = "Umberto Eco",
            Libri = new List<Libro>()
        }
    };

    public static List<Libro> Libri = new()
    {
        new Libro
        {
            Id = 1,
            Titolo = "Il barone rampante",
            AutoreId = 1,
            Autore = Autori[0]
        },
        new Libro
        {
            Id = 2,
            Titolo = "Il nome della rosa",
            AutoreId = 2,
            Autore = Autori[1]
        }
    };

    static MockData()
    {
        Autori[0].Libro.Add(Libri[0]);
        Autori[1].Librio.Add(Libri[1]);
    }
}


app.Run();



