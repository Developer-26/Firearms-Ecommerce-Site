using System.ComponentModel.DataAnnotations;

namespace Gun_Site.Models
{
    public class CartModel
    {
        [Key] // Explicitly mark it as the primary key
        public int CartId { get; set; } // Unique identifier

        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Price * Quantity; // Computed property
    }
}