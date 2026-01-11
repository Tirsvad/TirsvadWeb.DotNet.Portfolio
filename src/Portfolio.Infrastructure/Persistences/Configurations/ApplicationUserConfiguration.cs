using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Persistences.Configurations;

/// <summary>
/// EntityTypeConfiguration for <see cref="ApplicationUser"/> entity.
/// <para>
/// <b>Inheriting XML Documentation with &lt;inheritdoc/&gt;:</b><br/>
/// Use <c>&lt;inheritdoc/&gt;</c> in derived configuration classes or overridden members to automatically inherit XML documentation from base classes or interfaces. This keeps documentation DRY and consistent.<br/>
/// <b>How to use:</b><br/>
/// Place <c>&lt;inheritdoc/&gt;</c> in the XML doc comment of a derived class or member.<br/>
/// <b>Why we have it:</b><br/>
/// It helps maintain up-to-date and consistent documentation, especially for large codebases with many overrides or interface implementations.
/// </para>
/// <example>
/// Example of inheriting documentation in a derived configuration:
/// <code>
/// /// &lt;inheritdoc/&gt;
/// public class MyUserConfig : ApplicationUserConfiguration
/// {
///     // ...
/// }
/// </code>
/// </example>
/// </summary>
public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<ApplicationUser> entity)
    {
        // Map to table "Users"
        _ = entity.ToTable("Users");
        // Set primary key
        _ = entity.HasKey(e => e.Id);
        // Optional one-to-one relationship to ClientCertificate
        _ = entity.HasOne(e => e.Certificate)
            .WithOne()
            .HasForeignKey<ApplicationUser>(e => e.CertificateId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
