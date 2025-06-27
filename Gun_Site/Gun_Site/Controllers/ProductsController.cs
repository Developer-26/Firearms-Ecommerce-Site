using Gun_Site.Data;
using Gun_Site.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Gun_Site.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext1 _context;
        private readonly ILogger<ProductsController> _logger;
        private readonly string _productImagesFolder;

        public ProductsController(AppDbContext1 context, ILogger<ProductsController> logger, IWebHostEnvironment env)
        {
            _context = context;
            _logger = logger;
            // Set the product images folder to wwwroot/images/products
            _productImagesFolder = Path.Combine(env.WebRootPath, "images", "products");
        }

        public IActionResult Index()
        {
            var products = _context.AdminAddedProducts.ToList();
            return View(products);
        }

        public IActionResult ByCategory(string category)
        {
            var products = _context.AdminAddedProducts
                .Where(p => p.Category == category)
                .ToList();

            return View("Index", products);
        }

        public IActionResult AnotherView()
        {
            var products = _context.AdminAddedProducts.ToList();
            return View(products); // Load Products View For admin changes
        }



        public IActionResult Edit(int id)
        {
            var products = _context.AdminAddedProducts.Find(id);

            if (products == null)
            {
                TempData["ErrorMessage"] = "Product not found.";
                _logger.LogWarning("Product with ID {Id} not found.", id);
                return RedirectToAction(nameof(Index));
            }

            var productDto = new ProductDto
            {
                ProductName = products.ProductName,
                ProductDescription = products.ProductDescription,
                Category = products.Category,
                Price = products.Price,
             
            };

            ViewData["ProductId"] = products.ProductID;
            ViewData["ImagePath"] = products.ImagePath;

            return View(productDto);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductDto productDto)
        {
            var product = _context.AdminAddedProducts.Find(id);

            if (product == null)
            {
                TempData["ErrorMessage"] = "Product not found.";
                _logger.LogWarning("Product with ID {Id} not found.", id);
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                // Update product properties
                product.ProductName = productDto.ProductName;
                product.ProductDescription = productDto.ProductDescription;
                product.Category = productDto.Category;
                product.Price = productDto.Price;

                // Image upload functionality
                if (productDto.ImageFilePath != null && productDto.ImageFilePath.Length > 0)
                {
                    try
                    {
                        // Ensure the products images folder exists
                        if (!Directory.Exists(_productImagesFolder))
                        {
                            Directory.CreateDirectory(_productImagesFolder);
                        }

                        // Save the new file locally
                        string fileName = $"{Guid.NewGuid()}_{productDto.ImageFilePath.FileName}";
                        string filePath = Path.Combine(_productImagesFolder, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await productDto.ImageFilePath.CopyToAsync(stream);
                        }

                        // Update image path
                        product.ImagePath = $"/images/products/{fileName}";
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occurred while uploading the new image.");
                        ModelState.AddModelError(string.Empty, "An error occurred while uploading the new image.");
                        ViewData["ProductId"] = product.ProductID;
                        ViewData["ImagePath"] = product.ImagePath;
                        return View(productDto);
                    }
                }
                else
                {
                    // Retain existing image path if no new image is uploaded
                    product.ImagePath = product.ImagePath;
                }

                // Save changes
                _context.Update(product);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Product updated successfully!";
                _logger.LogInformation("Product with ID {Id} updated successfully.", product.ProductID);

                return RedirectToAction(nameof(Index));
            }

            ViewData["ProductId"] = product.ProductID;
            ViewData["ImagePath"] = product.ImagePath;

            return View(productDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.AdminAddedProducts.FindAsync(id);
            if (product == null)
            {
                TempData["ErrorMessage"] = "Product not found.";
                _logger.LogWarning("Product with ID {Id} not found.", id);

                // Redirect to AnotherView.cshtml explicitly
                var products = _context.AdminAddedProducts.ToList();
                return View("Views/Products/AnotherView.cshtml", products);
            }

            try
            {
                // Delete the product's image if it exists in the directory
                var imagePath = Path.Combine(_productImagesFolder, Path.GetFileName(product.ImagePath.TrimStart('/')));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                // Delete the product from the database
                _context.AdminAddedProducts.Remove(product);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Product deleted successfully!";
                _logger.LogInformation("Product with ID {Id} deleted successfully.", id);

                // Redirect to AnotherView.cshtml explicitly
                var updatedProducts = _context.AdminAddedProducts.ToList();
                return View("Views/Products/AnotherView.cshtml", updatedProducts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the product.");
                TempData["ErrorMessage"] = "An error occurred while deleting the product.";

                // Redirect to AnotherView.cshtml explicitly
                var products = _context.AdminAddedProducts.ToList();
                return View("Views/Products/AnotherView.cshtml", products);
            }
        }
        public IActionResult GridView(string category)
        {
            var products = _context.AdminAddedProducts
                .Where(p => p.Category == category)
                .ToList();

            ViewData["Category"] = category; // Pass category name to the view

            return View("Grid_Products", products); // Ensure it matches the actual file name
        }

    }
}
