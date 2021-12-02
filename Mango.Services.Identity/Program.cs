using Mango.Services.Identity;
using Mango.Services.Identity.DbContexts;
using Mango.Services.Identity.Initializer;
using Mango.Services.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

void AddServices(IServiceCollection service, ConfigurationManager configuration)
{
    // Add services to the container.
    service.AddControllersWithViews();

    service.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
    });
    service.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

    var identityBuilder = service.AddIdentityServer(options =>
        {
            options.Events.RaiseErrorEvents = true;
            options.Events.RaiseInformationEvents = true;
            options.Events.RaiseFailureEvents = true;
            options.Events.RaiseSuccessEvents = true;
            options.EmitStaticAudienceClaim = true;
        })
        .AddInMemoryIdentityResources(SD.IdentityResources)
        .AddInMemoryApiScopes(SD.ApiScopes)
        .AddInMemoryClients(SD.Client)
        .AddAspNetIdentity<ApplicationUser>();

    service.AddScoped<IDbInitializer, DbInitializer>();

    identityBuilder.AddDeveloperSigningCredential();
}

void Configure(WebApplication app)
{
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseIdentityServer();
    app.UseAuthorization();
    ConfigureAppServices(app);

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}

void ConfigureAppServices(IApplicationBuilder app)
{
    using var scope = app.ApplicationServices.CreateScope();
    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    dbInitializer.Initialize();
}

var builder = WebApplication.CreateBuilder(args);
AddServices(builder.Services, builder.Configuration);

var app = builder.Build();
Configure(app);


