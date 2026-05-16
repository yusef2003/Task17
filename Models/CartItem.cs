namespace CinemaDashboard.Models
{
    public class CartItem
    {
        public int MovieId    { get; set; }
        public string MovieName { get; set; } = string.Empty;
        public decimal Price   { get; set; }
        public int Quantity    { get; set; }
        public string MainImg  { get; set; } = string.Empty;
    }
}
