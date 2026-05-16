namespace CinemaDashboard.ViewModels
{
    public class FilterCustomerMovieVM
    {
        public string? MovieName { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public int CategoryId { get; set; }
        public int CinemaId { get; set; }
        public bool IsUpcoming { get; set; }
        public int Page { get; set; } = 1;
    }
}
