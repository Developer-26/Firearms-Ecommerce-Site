using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Gun_Site.Data;
using Gun_Site.Areas.Identity.Data;
using Gun_Site.Models;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Register AppDbContext with the connection string from appsettings.json
builder.Services.AddDbContext<Gun_Site.Data.AppDbContext1>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext1>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

try
{
    using (var scope = app.Services.CreateScope())
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var roles = new[] { "Admin", "User" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole(role));
                if (!roleResult.Succeeded)
                {
                    throw new Exception($"Failed to create role '{role}': {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                }
            }
        }

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext1>();


        // Dummy admin account created. Please note this is only for development purposes
        string email = "admin@admin.com";
        string password = "Test@1234";

        if (await userManager.FindByEmailAsync(email) == null)
        {
            var user = new ApplicationUser { FirstName = "Admin", LastName = "Admin", UserName = email, Email = email, EmailConfirmed = true };
            var createUserResult = await userManager.CreateAsync(user, password);

            if (createUserResult.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Admin");

                var admin = new AdministratorModel
                {
                    Name = "Admin",
                    Surname = "Admin",
                    Email = email,
                    Password = password,
                    ConfirmPassword = password
                };

                dbContext.Administrators.Add(admin);
                await dbContext.SaveChangesAsync();
            }
            else
            {
                throw new Exception($"Failed to create user '{email}': {string.Join(", ", createUserResult.Errors.Select(e => e.Description))}");
            }
        }
    }
}
catch (Exception ex)
{
    File.AppendAllText("error_log.txt", $"{DateTime.Now}: {ex.Message}{Environment.NewLine}");
}


app.Run();
