using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Gun_Site.Areas.Identity.Data;
using Gun_Site.Models;
using Gun_Site.Data;

namespace Gun_Site.Controllers
{
    public class AdministratorController : Controller
    {
        private readonly AppDbContext1 _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdministratorController(AppDbContext1 context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Administrator
        public async Task<IActionResult> Index()
        {
            return View(await _context.Administrators.ToListAsync());
        }

        // GET: Administrator/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var administratorModel = await _context.Administrators
                .FirstOrDefaultAsync(m => m.AdminId == id);
            if (administratorModel == null)
            {
                return NotFound();
            }

            return View(administratorModel);
        }

        // GET: Administrator/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Administrator/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AdminId,Name,Surname,Email,Password,ConfirmPassword")] AdministratorModel administratorModel)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = administratorModel.Email,
                    Email = administratorModel.Email,
                    FirstName = administratorModel.Name,
                    LastName = administratorModel.Surname
                };

                var result = await _userManager.CreateAsync(user, administratorModel.Password);
                if (result.Succeeded)
                {
                    var addToRoleResult = await _userManager.AddToRoleAsync(user, "Admin");
                    if (!addToRoleResult.Succeeded)
                    {
                        throw new Exception($"Failed to add user '{administratorModel.Email}' to role 'Admin': {string.Join(", ", addToRoleResult.Errors.Select(e => e.Description))}");
                    }

                    // Ensure ConfirmPassword is set and matches Password
                    administratorModel.ConfirmPassword = administratorModel.Password;

                    _context.Add(administratorModel);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(administratorModel);
        }

        //// POST: Administrator/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("AdminId,Name,Surname,Email,Password,ConfirmPassword")] AdministratorModel administratorModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = new ApplicationUser
        //        {
        //            UserName = administratorModel.Email,
        //            Email = administratorModel.Email,
        //            FirstName = administratorModel.Name,
        //            LastName = administratorModel.Surname
        //        };

        //        var result = await _userManager.CreateAsync(user, administratorModel.Password);
        //        if (result.Succeeded)
        //        {
        //            var addToRoleResult = await _userManager.AddToRoleAsync(user, "Admin");
        //            if (!addToRoleResult.Succeeded)
        //            {
        //                throw new Exception($"Failed to add user '{administratorModel.Email}' to role 'Admin': {string.Join(", ", addToRoleResult.Errors.Select(e => e.Description))}");
        //            }

        //            _context.Add(administratorModel);
        //            await _context.SaveChangesAsync();
        //            return RedirectToAction(nameof(Index));
        //        }
        //        else
        //        {
        //            foreach (var error in result.Errors)
        //            {
        //                ModelState.AddModelError(string.Empty, error.Description);
        //            }
        //        }
        //    }
        //    return View(administratorModel);
        //}

        // GET: Administrator/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var administratorModel = await _context.Administrators.FindAsync(id);
            if (administratorModel == null)
            {
                return NotFound();
            }
            return View(administratorModel);
        }

        // POST: Administrator/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AdminId,Name,Surname,Email")] AdministratorModel administratorModel)
        {
            if (id != administratorModel.AdminId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(administratorModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdministratorModelExists(administratorModel.AdminId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(administratorModel);
        }

        // GET: Administrator/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var administratorModel = await _context.Administrators
                .FirstOrDefaultAsync(m => m.AdminId == id);
            if (administratorModel == null)
            {
                return NotFound();
            }

            return View(administratorModel);
        }

        // POST: Administrator/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var administratorModel = await _context.Administrators.FindAsync(id);
            if (administratorModel != null)
            {
                var user = await _userManager.FindByEmailAsync(administratorModel.Email);
                if (user != null)
                {
                    var result = await _userManager.DeleteAsync(user);
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to delete user '{administratorModel.Email}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }

                _context.Administrators.Remove(administratorModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool AdministratorModelExists(int id)
        {
            return _context.Administrators.Any(e => e.AdminId == id);
        }
    }
}
