namespace Mango.Services.ShoppingCartAPI.DbContext;

using Microsoft.EntityFrameworkCore;
using Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<CartHeader> Header { get; set; }
    public DbSet<CartDetails> Details { get; set; }
}