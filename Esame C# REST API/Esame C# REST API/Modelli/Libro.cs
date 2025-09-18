namespace Esame_C__REST_API.Modelli
{
    public class Libro
    {
        public int Id { get; set; }
        public string Titolo { get; set; } = null!;

        public int AutoreId { get; set; }
        public Autore Autore { get; set; } = null!;
    }
}
