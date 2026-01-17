using LentzCraftServices.Components;
using LentzCraftServices.Infrastructure.Data;
using LentzCraftServices.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "LentzCraftServices")
    .WriteTo.Console()
    .WriteTo.File("logs/lentzcrafts-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Starting LentzCraftServices application");

// Use Serilog for logging
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure Entity Framework Core with SQLite (or production database from connection string)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
    ?? "Data Source=lentzcrafts.db";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Configure ASP.NET Core Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // Password settings - production ready
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    
    // User settings
    options.User.RequireUniqueEmail = true;
    
    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// Configure authentication cookie with security hardening
var isProduction = builder.Environment.IsProduction();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
    
    // Security hardening
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = isProduction 
        ? Microsoft.AspNetCore.Http.CookieSecurePolicy.Always 
        : Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
});

// Rate limiting is handled by Identity's lockout feature
// Additional rate limiting can be added via middleware if needed

// Add health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

// Add memory cache
builder.Services.AddMemoryCache();

// Register repositories with caching
builder.Services.AddScoped<ProductRepository>();
builder.Services.AddScoped<IProductRepository>(serviceProvider =>
{
    var innerRepository = serviceProvider.GetRequiredService<ProductRepository>();
    var cache = serviceProvider.GetRequiredService<IMemoryCache>();
    var logger = serviceProvider.GetRequiredService<ILogger<CachedProductRepository>>();
    return new CachedProductRepository(innerRepository, cache, logger);
});

// Add HttpContextAccessor for server-side redirects
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Initialize database and seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var configuration = services.GetRequiredService<IConfiguration>();
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    // Apply migrations in production, ensure created in development
    if (app.Environment.IsDevelopment())
    {
        await context.Database.EnsureCreatedAsync();
    }
    else
    {
        // In production, migrations should be applied via CI/CD or manually
        // Uncomment the line below if you want automatic migration application
        // await context.Database.MigrateAsync();
        logger.LogInformation("Database migrations should be applied manually in production");
    }
    
    await DbInitializer.InitializeAsync(context, userManager, configuration, logger);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // HSTS: 365 days for production
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

// Add security headers
app.Use(async (context, next) =>
{
    if (!app.Environment.IsDevelopment())
    {
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("X-Frame-Options", "DENY");
        context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
        context.Response.Headers.Append("Content-Security-Policy", 
            "default-src 'self'; " +
            "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
            "style-src 'self' 'unsafe-inline'; " +
            "img-src 'self' data: https:; " +
            "font-src 'self' data:; " +
            "connect-src 'self';");
    }
    await next();
});

app.UseHttpsRedirection();

// Configure static files with caching
var staticFileOptions = new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        if (!app.Environment.IsDevelopment())
        {
            // Cache static files for 1 year
            ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=31536000");
        }
    }
};
app.UseStaticFiles(staticFileOptions);

app.UseAntiforgery();

// Add global exception handler
app.UseMiddleware<LentzCraftServices.Middleware.GlobalExceptionHandlerMiddleware>();

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Health checks endpoint
app.MapHealthChecks("/health");

// Sitemap endpoint
app.MapGet("/sitemap.xml", async (
    IProductRepository productRepository,
    HttpContext context) =>
{
    try
    {
        var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";
        var products = await productRepository.GetPublicProductsAsync();
        
        var sitemap = new System.Text.StringBuilder();
        sitemap.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        sitemap.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");
        
        // Home page
        sitemap.AppendLine("  <url>");
        sitemap.AppendLine($"    <loc>{baseUrl}/</loc>");
        sitemap.AppendLine("    <changefreq>weekly</changefreq>");
        sitemap.AppendLine("    <priority>1.0</priority>");
        sitemap.AppendLine("  </url>");
        
        // Products page
        sitemap.AppendLine("  <url>");
        sitemap.AppendLine($"    <loc>{baseUrl}/products</loc>");
        sitemap.AppendLine("    <changefreq>weekly</changefreq>");
        sitemap.AppendLine("    <priority>0.8</priority>");
        sitemap.AppendLine("  </url>");
        
        // About page
        sitemap.AppendLine("  <url>");
        sitemap.AppendLine($"    <loc>{baseUrl}/about</loc>");
        sitemap.AppendLine("    <changefreq>monthly</changefreq>");
        sitemap.AppendLine("    <priority>0.6</priority>");
        sitemap.AppendLine("  </url>");
        
        // Contact page
        sitemap.AppendLine("  <url>");
        sitemap.AppendLine($"    <loc>{baseUrl}/contact</loc>");
        sitemap.AppendLine("    <changefreq>monthly</changefreq>");
        sitemap.AppendLine("    <priority>0.6</priority>");
        sitemap.AppendLine("  </url>");
        
        // Product detail pages
        foreach (var product in products)
        {
            sitemap.AppendLine("  <url>");
            sitemap.AppendLine($"    <loc>{baseUrl}/products/{product.Id}</loc>");
            sitemap.AppendLine("    <changefreq>monthly</changefreq>");
            sitemap.AppendLine("    <priority>0.7</priority>");
            sitemap.AppendLine("  </url>");
        }
        
        sitemap.AppendLine("</urlset>");
        
        return Results.Content(sitemap.ToString(), "application/xml");
    }
    catch (Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error generating sitemap");
        return Results.StatusCode(500);
    }
})
.AllowAnonymous();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Login endpoint to handle authentication (avoids header issues in Blazor Server)
app.MapPost("/Account/LoginPost", async (
    HttpRequest request,
    SignInManager<IdentityUser> signInManager) =>
{
    var form = await request.ReadFormAsync();
    var email = form["email"].ToString()?.Trim() ?? string.Empty;
    var password = form["password"].ToString() ?? string.Empty;
    var rememberMe = form.ContainsKey("rememberMe") && bool.TryParse(form["rememberMe"].ToString(), out var rm) && rm;
    var returnUrl = form["returnUrl"].ToString() ?? "/admin";
    
    // Sanitize returnUrl to prevent open redirect attacks
    if (!string.IsNullOrEmpty(returnUrl) && !returnUrl.StartsWith("/"))
    {
        returnUrl = "/admin";
    }
    
    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
    {
        return Results.Redirect($"/Account/Login?error={Uri.EscapeDataString("Email and password are required.")}");
    }
    
    // Enable lockout on failure for security
    var result = await signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: true);
    
    if (result.Succeeded)
    {
        return Results.Redirect(returnUrl);
    }
    
    if (result.IsLockedOut)
    {
        return Results.Redirect($"/Account/Login?error={Uri.EscapeDataString("Account locked out due to multiple failed login attempts. Please try again later.")}");
    }
    
    return Results.Redirect($"/Account/Login?error={Uri.EscapeDataString("Invalid login attempt. Please check your email and password.")}");
})
.AllowAnonymous();

app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}