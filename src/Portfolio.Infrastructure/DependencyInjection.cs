using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Portfolio.Core;
using Portfolio.Core.Abstracts;
using Portfolio.Infrastructure.Abstacts;
using Portfolio.Infrastructure.Persistences;
using Portfolio.Infrastructure.Repositories;
using Portfolio.Infrastructure.Services;

namespace Portfolio.Infrastructure;

/// <summary>
/// Provides extension methods for registering infrastructure services.
/// </summary>
/// <remarks>
/// <para>
/// This static class contains methods to register infrastructure-related services, such as database context, repositories, and helpers, into the dependency injection container.
/// </para>
/// <para>
/// <b>Inheriting XML Documentation with &lt;inheritdoc/&gt;:</b><br/>
/// The <c>&lt;inheritdoc/&gt;</c> tag allows derived classes or interface implementations to inherit XML documentation from their base types or interfaces. This reduces duplication and ensures consistency in documentation.
/// </para>
/// <para>
/// <b>How to use:</b><br/>
/// Call <see cref="AddInfrastructureServices"/> in your application's service configuration to register all infrastructure dependencies.
/// </para>
/// <para>
/// <b>Why we have it:</b><br/>
/// Centralizing service registration improves maintainability and testability by keeping dependency configuration in one place.
/// </para>
/// <code language="csharp">
/// // Example usage in Program.cs or Startup.cs
/// var builder = WebAssemblyHostBuilder.CreateDefault(args);
/// builder.Services.AddInfrastructureServices(builder.Configuration);
/// </code>
/// </remarks>
public static class DependencyInjection
{
    #region Public Methods
    /// <summary>
    /// Registers infrastructure services into the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The application configuration.</param>
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCoreServices();

        string? env = GetEnvironment(configuration); // Get current environment
        string? userSecretsId = GetUserSecretsId(configuration); // Get user secrets ID

        AddDbContextServices(services, env, userSecretsId); // Register DbContext and related services
        services.AddScoped<IClientCertificateRepository, ClientCertificateRepository>();
        services.AddScoped<ICertificateSignInService, CertificateSignInService>();
    }
    #endregion

    #region Helpers
    /// <summary>
    /// Gets the current environment name from environment variables or configuration.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The environment name, or null if not found.</returns>
    private static string? GetEnvironment(IConfiguration configuration)
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
               ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
               ?? configuration["Environment"];
    }

    /// <summary>
    /// Gets the user secrets ID from configuration.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The user secrets ID, or null if not found.</returns>
    private static string? GetUserSecretsId(IConfiguration configuration)
    {
        return configuration["Portfolio:UserSecretsId"]
               ?? configuration["UserSecrets:Id"];
    }

    /// <summary>
    /// Registers the application's DbContext and related services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="env">The environment name.</param>
    /// <param name="userSecretsId">The user secrets ID.</param>
    private static void AddDbContextServices(IServiceCollection services, string? env, string? userSecretsId)
    {
        // Register helper for DbContext options
        services.AddScoped<IDbContextOptionsServices>(_ => new DbContextOptionsServices(userSecretsId));
        // Register ApplicationDbContext with connection string
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            IDbContextOptionsServices optsSvc = sp.GetRequiredService<IDbContextOptionsServices>();
            string conn = optsSvc.CreateConnectionString(env ?? string.Empty);
            options.UseSqlServer(conn, sqlOptions => sqlOptions.EnableRetryOnFailure());
        });
    }
    #endregion
}
