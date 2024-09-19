using DecorVista.DataAccess.Data;
using DecorVista.DataAccess.Repository;
using DecorVista.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;
using DecorVista.DataAccess.DbInitializer;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Hosting;
using DecorVista.Utility;
using BulkyBook.DataAccess.DbInitializer;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // Add services to the container.
        services.AddControllersWithViews();

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
                   .EnableSensitiveDataLogging(); // Enables detailed logging of SQL queries
        });

        // Configure Stripe
        services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));

        // Configure Identity services with IdentityUser
        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        // Configure application cookies
        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Identity/Account/Login";
            options.LogoutPath = "/Identity/Account/Logout";
            options.AccessDeniedPath = "/Identity/Account/AccessDenied";
        });

        // Configure Facebook Authentication
        services.AddAuthentication().AddFacebook(options =>
        {
            options.AppId = "815424026933393";
            options.AppSecret = "dfc3081bc8bde54f0f2959077f3b3438";
        });

        // Configure distributed memory cache and session
        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(100);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        // Register custom services
        services.AddScoped<IDbInitializer, DbInitializer>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IEmailSender, EmailSender>();

        services.AddRazorPages();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Configure the HTTP request pipeline.
        if (!env.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts(); // Enforces HTTPS
        }
        else
        {
            app.UseDeveloperExceptionPage(); // Adds a developer exception page in development
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        // Configure Stripe
        StripeConfiguration.ApiKey = Configuration.GetSection("Stripe:SecretKey").Value;

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSession();

        // Seed the database
        SeedDatabase(app);

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

            endpoints.MapRazorPages();
        });
    }

    private void SeedDatabase(IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
            dbInitializer.Initialize(); // Initialize the database with roles and default users
        }
    }
}
