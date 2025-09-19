namespace Esame_c__REST_API_Autore_libro.Modelli
{
    public class Libro
    {
        public int LibroId { get; set; }
        public string Titolo { get; set; } = null!;
        public int AutoreId { get; set; }
        public int AnnoPubblicazione { get; set; }

        public Autore Autore { get; set; } = null!;
    }
}
