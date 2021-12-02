namespace Mango.Services.Identity.Initializer;

using System.Security.Claims;
using DbContexts;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Models;

public class DbInitializer : IDbInitializer
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    public DbInitializer(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _db = db;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public void Initialize()
    {
        if (_roleManager.FindByIdAsync(SD.Admin).Result == null)
        {
            _roleManager.CreateAsync(new IdentityRole(SD.Admin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Customer)).GetAwaiter().GetResult();
        }
        else
        {
            return;
        }

        var adminUser = new ApplicationUser()
        {
            UserName = "admin1@mail.com",
            Email = "admin1@gmail.com",
            EmailConfirmed = true,
            PhoneNumber = "111111111",
            FirstName = "Iran",
            LastName = "Admin"
        };

        _userManager.CreateAsync(adminUser, "Admin123*").GetAwaiter().GetResult();
        _userManager.AddToRoleAsync(adminUser, SD.Admin).GetAwaiter().GetResult();

        var temp1 = _userManager.AddClaimsAsync(adminUser, new[]
        {
            new Claim(JwtClaimTypes.Name, adminUser.FirstName + " " + adminUser.LastName),
            new Claim(JwtClaimTypes.GivenName, adminUser.FirstName),
            new Claim(JwtClaimTypes.FamilyName, adminUser.FirstName),
            new Claim(JwtClaimTypes.Role, SD.Admin),
        }).Result;
        
        var customUser = new ApplicationUser()
        {
            UserName = "customer@mail.com",
            Email = "customer1@gmail.com",
            EmailConfirmed = true,
            PhoneNumber = "111111111",
            FirstName = "Mono",
            LastName = "Cust"
        };

        _userManager.CreateAsync(customUser, "Admin123*").GetAwaiter().GetResult();
        _userManager.AddToRoleAsync(customUser, SD.Customer).GetAwaiter().GetResult();

        var temp2 = _userManager.AddClaimsAsync(customUser, new Claim[]
        {
            new Claim(JwtClaimTypes.Name, customUser.FirstName + " " + customUser.LastName),
            new Claim(JwtClaimTypes.GivenName, customUser.FirstName),
            new Claim(JwtClaimTypes.FamilyName, customUser.FirstName),
            new Claim(JwtClaimTypes.Role, SD.Customer),
        }).Result;
    }
}