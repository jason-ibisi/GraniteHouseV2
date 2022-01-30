using GraniteHouseV2_Models;
using GraniteHouseV2_Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace GraniteHouseV2_DataAccess.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private IConfiguration configuration { get; }

        public DbInitializer(ApplicationDbContext db, UserManager<IdentityUser> userManager, 
            RoleManager<IdentityRole> roleManager, IConfiguration Configuration)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
            configuration = Configuration;
        }

        public void Initialize()
        {
            string _username = configuration.GetSection("AdminUser").GetValue<string>("UserName");
            string _email = configuration.GetSection("AdminUser").GetValue<string>("Email");
            string _password = configuration.GetSection("AdminUser").GetValue<string>("Password");

            // Apply any pending migrations
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch(Exception ex)
            {

            }

            // Create roles
            if (!_roleManager.RoleExistsAsync(AppConstants.AdminRole).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(AppConstants.AdminRole)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(AppConstants.CustomerRole)).GetAwaiter().GetResult();
            }
            else
            {
                return;
            }

            // Create user 
            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = _username,
                Email = _email,
                EmailConfirmed = true,
                FullName = "Davey Jones",
                PhoneNumber = "09012312311",
                PhoneNumberConfirmed = true
            }, _password).GetAwaiter().GetResult();

            // Assign user the role of admin
            ApplicationUser user = _db.ApplicationUser.FirstOrDefault(u => u.Email == _email);
            _userManager.AddToRoleAsync(user, AppConstants.AdminRole).GetAwaiter().GetResult();
        }
    }
}
