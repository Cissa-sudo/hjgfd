namespace SpaApp.Models
{
    public class Subscription
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
        public double Rating { get; set; }
        public string FacilityName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Features { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public int VisitsCount { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsInComparison { get; set; }
    }
}