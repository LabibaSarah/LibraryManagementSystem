using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace LibraryManagementSystem
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add database context
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));


            // =========================================================
            // PERSIST IDENTITY AUTHENTICATION KEYS
            // =========================================================

            builder.Services
                .AddDataProtection()
                .PersistKeysToFileSystem(
                    new DirectoryInfo(
                        Path.Combine(
                            builder.Environment.ContentRootPath,
                            "DataProtectionKeys"
                        )))
                .SetApplicationName("LibraryManagementSystem");


            // =========================================================
            // ADD ASP.NET CORE IDENTITY
            // =========================================================

            builder.Services
                .AddDefaultIdentity<LibraryManagementSystem.Data.ApplicationUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();


            // Add MVC services
            builder.Services.AddControllersWithViews();


            var app = builder.Build();


            // =========================================================
            // SEED ROLES AND DEFAULT ADMIN
            // =========================================================

            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider
                    .GetRequiredService<RoleManager<IdentityRole>>();

                var userManager = scope.ServiceProvider
                    .GetRequiredService<UserManager<LibraryManagementSystem.Data.ApplicationUser>>();

                // Create roles
                await DbInitializer.SeedRolesAsync(roleManager);

                // Create default Admin account
                await DbInitializer.SeedAdminAsync(userManager);
            }


            // =========================================================
            // CONFIGURE HTTP REQUEST PIPELINE
            // =========================================================

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();


            // Authentication
            app.UseAuthentication();

            // Authorization
            app.UseAuthorization();


            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.MapRazorPages();


            app.Run();
        }
    }
}