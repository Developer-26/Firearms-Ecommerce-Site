using Gun_Site.Data;
using Gun_Site.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Gun_Site.Controllers
{
    public class ProductSpecialsController : Controller
    {
        private readonly AppDbContext1 _context;
        private readonly ILogger<ProductSpecialsController> _logger;
        private readonly string _specialsImagesFolder;

        public ProductSpecialsController(AppDbContext1 context, ILogger<ProductSpecialsController> logger, IWebHostEnvironment env)
        {
            _context = context;
            _logger = logger;
            _specialsImagesFolder = Path.Combine(env.WebRootPath, "images", "products");
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductSpecialsDto productSpecialsDto)
        {
            if (productSpecialsDto.ImageFilePath == null || productSpecialsDto.ImageFilePath.Length == 0)
            {
                ModelState.AddModelError("ImageFilePath", "The image file is required.");
                _logger.LogWarning("Specials image file is null or empty.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!Directory.Exists(_specialsImagesFolder))
                    {
                        Directory.CreateDirectory(_specialsImagesFolder);
                    }

                    string fileName = $"{Guid.NewGuid()}_{productSpecialsDto.ImageFilePath.FileName}";

                    string filePath = Path.Combine(_specialsImagesFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await productSpecialsDto.ImageFilePath.CopyToAsync(stream);
                    }

                    var productSpecial = new ProductSpecials
                    {
                        ProductName = productSpecialsDto.ProductName,
                        ProductDescription = productSpecialsDto.ProductDescription,
                        Price = productSpecialsDto.Price,
                        ImagePath = $"/images/products/{fileName}",
                        Category = productSpecialsDto.Category
                    };

                    _context.ProductSpecials.Add(productSpecial);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Special product created successfully!";
                    _logger.LogInformation("Special product created successfully with ID: {SpecialID}", productSpecial.SpecialID);

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while uploading the special product image.");
                    ModelState.AddModelError(string.Empty, "An error occurred while uploading the image.");
                    TempData["ErrorMessage"] = "An error occurred while uploading the image.";
                }
            }

            return View(productSpecialsDto);
        }

        public IActionResult Index()
        {
            var productSpecials = _context.ProductSpecials.ToList();
            return View(productSpecials);
        }

        public IActionResult Edit(int id)
        {
            var productSpecial = _context.ProductSpecials.Find(id);

            if (productSpecial == null)
            {
                TempData["ErrorMessage"] = "Special product not found.";
                _logger.LogWarning("Special product with ID {Id} not found.", id);
                return RedirectToAction(nameof(Index));
            }

            var productSpecialDto = new ProductSpecialsDto
            {
                ProductName = productSpecial.ProductName,
                ProductDescription = productSpecial.ProductDescription,
                Category = productSpecial.Category,
                Price = productSpecial.Price
            };

            ViewData["SpecialID"] = productSpecial.SpecialID;
            ViewData["ImagePath"] = productSpecial.ImagePath;

            return View(productSpecialDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductSpecialsDto productSpecialsDto)
        {
            var productSpecial = _context.ProductSpecials.Find(id);

            if (productSpecial == null)
            {
                TempData["ErrorMessage"] = "Special product not found.";
                _logger.LogWarning("Special product with ID {Id} not found.", id);
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                // Update product properties
                productSpecial.ProductName = productSpecialsDto.ProductName;
                productSpecial.ProductDescription = productSpecialsDto.ProductDescription;
                productSpecial.Category = productSpecialsDto.Category;
                productSpecial.Price = productSpecialsDto.Price;

                // Image upload functionality
                if (productSpecialsDto.ImageFilePath != null && productSpecialsDto.ImageFilePath.Length > 0)
                {
                    try
                    {
                        // Ensure the specials images folder exists
                        if (!Directory.Exists(_specialsImagesFolder))
                        {
                            Directory.CreateDirectory(_specialsImagesFolder);
                        }

                        // Save the new file locally
                        string fileName = $"{Guid.NewGuid()}_{productSpecialsDto.ImageFilePath.FileName}";
                        string filePath = Path.Combine(_specialsImagesFolder, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await productSpecialsDto.ImageFilePath.CopyToAsync(stream);
                        }

                        // Update image path
                        productSpecial.ImagePath = $"/images/products/{fileName}";
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occurred while uploading the new image.");
                        ModelState.AddModelError(string.Empty, "An error occurred while uploading the new image.");
                        ViewData["SpecialID"] = productSpecial.SpecialID;
                        ViewData["ImagePath"] = productSpecial.ImagePath;
                        return View(productSpecialsDto);
                    }
                }
                else
                {
                    // Retain existing image path if no new image is uploaded
                    productSpecial.ImagePath = productSpecial.ImagePath;
                }

                // Save changes
                _context.Update(productSpecial);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Special product updated successfully!";
                _logger.LogInformation("Special product with ID {Id} updated successfully.", productSpecial.SpecialID);

                return RedirectToAction(nameof(Index));
            }

            ViewData["SpecialID"] = productSpecial.SpecialID;
            ViewData["ImagePath"] = productSpecial.ImagePath;

            return View(productSpecialsDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var productSpecial = await _context.ProductSpecials.FindAsync(id);
            if (productSpecial == null)
            {
                TempData["ErrorMessage"] = "Special product not found.";
                _logger.LogWarning("Special product with ID {Id} not found.", id);
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // Delete the product's image if it exists
                if (!string.IsNullOrEmpty(productSpecial.ImagePath))
                {
                    var imageFileName = Path.GetFileName(productSpecial.ImagePath);
                    var imagePath = Path.Combine(_specialsImagesFolder, imageFileName);

                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                        _logger.LogInformation("Deleted image file: {ImagePath}", imagePath);
                    }
                }

                // Delete the product special from the database
                _context.ProductSpecials.Remove(productSpecial);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Special product deleted successfully!";
                _logger.LogInformation("Special product with ID {Id} deleted successfully.", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the special product.");
                TempData["ErrorMessage"] = "An error occurred while deleting the special product.";
            }

            return RedirectToAction(nameof(Index));
        }


    }
}
