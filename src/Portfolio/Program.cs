using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Portfolio.Components;
using Portfolio.Components.Account;
using Portfolio.Domain.Entities;
using Portfolio.Infrastructure;
using Portfolio.Infrastructure.Persistences;
using Portfolio.Middleware;

using System.Reflection;

//using Portfolio.Components;
//using Portfolio.Components.Account;
//using Portfolio.Infrastructure;
//using Portfolio.Infrastructure.Persistents;
//using Portfolio.Middleware;


namespace Portfolio;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        try
        {
            _ = builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true, reloadOnChange: false);
        }
        catch
        {
            // ignore if user secrets are not available in this environment
        }

        // If running in Docker/tests-stage the compose file sets DB_PORTFOLIO_HOST=host.docker.internal
        // prefer that value for connecting to the host's database from inside the container
        //string? dockerDbHost = Environment.GetEnvironmentVariable("DB_PORTFOLIO_HOST");
        //if (!string.IsNullOrWhiteSpace(dockerDbHost))
        //{
        //    builder.Configuration["Database:Host"] = dockerDbHost;
        //}

        // Configure data protection key persistence so keys survive process/container restarts.
        // Path can be overridden by configuration: "DataProtection:KeyPath" (useful for Docker volumes).
        string? dpPath = builder.Configuration["DataProtection:KeyPath"] ?? builder.Configuration["DataProtection__KeyPath"];
        if (string.IsNullOrWhiteSpace(dpPath))
        {
            dpPath = Path.Combine(builder.Environment.ContentRootPath, "keys");
        }

        try
        {
            _ = Directory.CreateDirectory(dpPath!);
            _ = builder.Services.AddDataProtection()
                .SetApplicationName("Portfolio")
                .PersistKeysToFileSystem(new DirectoryInfo(dpPath!));
        }
        catch (Exception ex)
        {
            // Surface an explicit error so Docker / logs show why keys aren't persisted.
            Console.Error.WriteLine($"WARNING: Failed to persist data-protection keys to '{dpPath}'. Falling back to in-memory key ring. Ensure the path is mounted and writable. Exception: {ex.Message}");
        }

        //// Add services to the container.
        _ = builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents()
            .AddAuthenticationStateSerialization();

        _ = builder.Services.AddCascadingAuthenticationState();
        _ = builder.Services.AddScoped<IdentityRedirectManager>();
        _ = builder.Services.AddScoped<AuthenticationStateProvider, PersistingServerAuthenticationStateProvider>();

        // register certificate service and infrastructure (EF Core)
        builder.Services.AddInfrastructureServices(builder.Configuration);

        _ = builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        })
        .AddIdentityCookies();

        // Register cookie authentication so we can establish an authenticated session from a client certificate
        _ = builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "Portfolio.Auth";
                options.LoginPath = "/login";
                options.Cookie.SameSite = SameSiteMode.Lax;
            });

        _ = builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        _ = builder.Services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
            options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
        })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        _ = builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            //app.UseWebAssemblyDebugging();
            _ = app.UseMigrationsEndPoint();
        }
        else
        {
            _ = app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            _ = app.UseHsts();
        }

        _ = app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);


        // Only enable HTTPS redirection if an HTTPS endpoint is configured.
        // This avoids the runtime warning when the container is serving HTTP only (common in Docker setups)
        // Moved to extension method to keep Program.cs minimal
        app.UseConditionalHttpsRedirection(builder.Configuration);

        _ = app.UseAntiforgery();

        _ = app.MapStaticAssets();



        //// Apply any pending migrations at startup
        //using (IServiceScope scope = app.Services.CreateScope())
        //{
        //    ApplicationDbContext db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        //    //db.Database.Migrate();
        //}

        // Middleware: attempt to autoload a predefined X509 certificate (if present) and authenticate the user
        _ = app.UseMiddleware<PreloadedX509CertificateMiddleware>();

        _ = app.MapStaticAssets();
        _ = app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

        // Add additional endpoints required by the Identity /Account Razor components.
        _ = app.MapAdditionalIdentityEndpoints();

        app.Run();
    }
}
