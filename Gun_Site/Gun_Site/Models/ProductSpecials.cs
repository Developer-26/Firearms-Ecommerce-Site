using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gun_Site.Models
{
    public class ProductSpecials
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SpecialID { get; set; }

        [Required]
        public string ProductName { get; set; }

        [Required]
        public string ProductDescription { get; set; }

        [Required]
       
        public decimal Price { get; set; }  // Price with 2 decimal places

        public string ImagePath { get; set; }

        [Required]
        public string Category { get; set; }

       
    }
}
