namespace Esame_C__REST_API.Modelli
{
    public class Autore
    {
        public int Id { get; set; }

        public string Nome { get; set; } = null;

        public List<Libro> Libri { get; set; } = new();

    }
}
