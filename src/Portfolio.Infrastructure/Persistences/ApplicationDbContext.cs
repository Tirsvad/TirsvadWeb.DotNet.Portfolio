using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Portfolio.Domain.Entities;
using Portfolio.Infrastructure.Persistences.Configurations;

namespace Portfolio.Infrastructure.Persistences;

/// <summary>
/// The application's Entity Framework Core database context, including Identity and custom entities.
/// <para>
/// <b>Inheriting XML Documentation with &lt;inheritdoc/&gt;:</b><br/>
/// Use <c>&lt;inheritdoc/&gt;</c> in derived classes or overridden members to automatically inherit XML documentation from base classes or interfaces. This reduces duplication and ensures documentation stays up to date.<br/>
/// <b>How to use:</b><br/>
/// Place <c>&lt;inheritdoc/&gt;</c> in the XML doc comment of a derived class or member.<br/>
/// <b>Why we have it:</b><br/>
/// It helps maintain consistent and DRY documentation, especially in large codebases with many overrides or interface implementations.
/// </para>
/// <example>
/// Example of inheriting documentation in a derived context:
/// <code>
/// /// &lt;inheritdoc/&gt;
/// public class MyDbContext : ApplicationDbContext
/// {
///     // ...
/// }
/// </code>
/// </example>
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    #region properties
    /// <summary>
    /// Gets or sets the client certificates in the database.
    /// </summary>
    public DbSet<ClientCertificate> ClientCertificates { get; set; } = null!;
    #endregion

    #region constructors
    /// <inheritdoc/>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    #endregion

    #region overrides
    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply ApplicationUser configuration (see Configurations/ApplicationUserConfiguration.cs)
        _ = modelBuilder.ApplyConfiguration(new ApplicationUserConfiguration());
        // Apply ClientCertificate configuration (see Configurations/ClientCertificateConfiguration.cs)
        _ = modelBuilder.ApplyConfiguration(new ClientCertificateConfiguration());
    }
    #endregion
}