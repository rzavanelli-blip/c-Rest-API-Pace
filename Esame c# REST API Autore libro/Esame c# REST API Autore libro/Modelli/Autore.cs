namespace Esame_c__REST_API_Autore_libro.Modelli
{
    public class Autore
    {
        public int AutoreId { get; set; }
        public string Nome { get; set; }
        public List<Libro> Libri { get; set; } = new List<Libro>();
    }
}
