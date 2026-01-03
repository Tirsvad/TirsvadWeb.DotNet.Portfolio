using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Persistences.Configurations;

/// <summary>
/// EntityTypeConfiguration for <see cref="ClientCertificate"/> entity.
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
/// public class MyCertificateConfig : ClientCertificateConfiguration
/// {
///     // ...
/// }
/// </code>
/// </example>
/// </summary>
public class ClientCertificateConfiguration : IEntityTypeConfiguration<ClientCertificate>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<ClientCertificate> entity)
    {
        // Map to table "ClientCertificates"
        _ = entity.ToTable("ClientCertificates");
        // Set primary key
        _ = entity.HasKey(e => e.Id);
        // Configure required properties with max length
        _ = entity.Property(e => e.Subject)
            .IsRequired()
            .HasMaxLength(1024);
        _ = entity.Property(e => e.Issuer)
            .IsRequired()
            .HasMaxLength(1024);
        _ = entity.Property(e => e.SerialNumber)
            .IsRequired()
            .HasMaxLength(256);
    }
}
