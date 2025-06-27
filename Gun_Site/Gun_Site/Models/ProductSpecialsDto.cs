namespace Gun_Site.Models
{
    public class ProductSpecialsDto
    {
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public decimal Price { get; set; }
        public IFormFile? ImageFilePath { get; set; }
        public string Category { get; set; }
    }
}
