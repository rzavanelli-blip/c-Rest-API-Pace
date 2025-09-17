namespace Pizzeria_Rest_Api.Classi
{
    public class Pizza
    {
        public int Id { get; set; }
        public string? Nome { get; set; } = null!;
        public bool IsVegana { get; set; }
        public decimal Prezzo { get; set; }
        public List<Ingrediente> Ingredientes { get; set; } = new List<Ingrediente>();
    }

}
