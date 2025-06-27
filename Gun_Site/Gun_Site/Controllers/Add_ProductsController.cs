using Gun_Site.Data;
using Gun_Site.Models;
using Microsoft.AspNetCore.Mvc;
namespace Gun_Site.Controllers
{
    public class Add_ProductsController : Controller
    {
        private readonly AppDbContext1 _context;
        private readonly ILogger<Add_ProductsController> _logger;
        private readonly string _productImagesFolder;


        public Add_ProductsController(AppDbContext1 context, ILogger<Add_ProductsController> logger, IWebHostEnvironment env)
        {
            _context = context;
            _logger = logger;
            _productImagesFolder = Path.Combine(env.WebRootPath, "images", "products");
        }




        public IActionResult Create() => View();

        // POST: Project/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductDto productDto)
        {
            if (productDto.ImageFilePath == null || productDto.ImageFilePath.Length == 0)
            {
                ModelState.AddModelError("ImageFilePath", "The image file is required.");
                _logger.LogWarning("Image file is null or empty.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!Directory.Exists(_productImagesFolder))
                    {
                        Directory.CreateDirectory(_productImagesFolder);
                    }

                    string fileName = $"{Guid.NewGuid()}_{productDto.ImageFilePath.FileName}";
                    string filePath = Path.Combine(_productImagesFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await productDto.ImageFilePath.CopyToAsync(stream);
                    }

                    var product = new Admin_Added_Products
                    {
                        ProductName = productDto.ProductName,
                        ProductDescription = productDto.ProductDescription,
                        Price = productDto.Price,
                        ImagePath = $"/images/products/{fileName}",
                        Category = productDto.Category // Save category
                    };

                    _context.AdminAddedProducts.Add(product);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Product created successfully!";
                    _logger.LogInformation("Product created successfully with ID: {ProductId}", product.ProductID);

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while uploading the file.");
                    ModelState.AddModelError(string.Empty, "An error occurred while uploading the image.");
                    TempData["ErrorMessage"] = "An error occurred while uploading the image.";
                }
            }

            return View(productDto);
        }


        public IActionResult Index()
        {
            var products = _context.AdminAddedProducts.ToList();

            // Return the Index.cshtml view from the Products folder
            return View("Views/Products/Index.cshtml", products);
        }




    }
}