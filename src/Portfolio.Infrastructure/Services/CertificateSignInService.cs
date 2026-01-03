using Portfolio.Core.Abstracts;
using Portfolio.Domain.Entities;

using System.Collections.Concurrent;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace Portfolio.Infrastructure.Services;

/// <summary>
/// <inheritdoc />
/// </summary>
/// <remarks>
/// This implementation inherits its contract documentation from <see cref="ICertificateSignInService" /> using
/// <c>&lt;inheritdoc/&gt;</c>. Inheriting documentation reduces duplication and keeps comments consistent with the
/// interface when the contract changes.
/// 
/// How to use:
/// - Register the service with dependency injection (e.g. <c>builder.Services.AddScoped&lt;ICertificateSignInService, CertificateSignInService&gt;()</c>).
/// - Call <see cref="CreatePrincipalForCertificateAsync"/> with an <see cref="X509Certificate2"/> to obtain a
///   <see cref="ClaimsPrincipal"/> representing the certificate subject.
/// 
/// Why we have it:
/// - Provides a central place to transform a client certificate into an application principal and to persist
///   certificate metadata when first seen.
/// - Using <c>&lt;inheritdoc/&gt;</c> keeps interface and implementation documentation synchronized.
/// </remarks>
/// <example>
/// Example usage:
/// <code language="csharp">
/// // Register in DI (Program.cs)
/// builder.Services.AddScoped&lt;ICertificateSignInService, CertificateSignInService&gt;();
///
///
/// // Resolve and use
/// var certService = serviceProvider.GetRequiredService&lt;ICertificateSignInService&gt;();
/// var cert = new X509Certificate2("path/to/cert.pfx", "password");
/// var principal = await certService.CreatePrincipalForCertificateAsync(cert);
/// if (principal != null)
/// {
///     // Sign in or use the principal
/// }
/// </code>
/// </example>
public class CertificateSignInService : ICertificateSignInService
{
    private readonly IClientCertificateRepository _repo;
    private const string CookieScheme = "Cookies";
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> _subjectLocks = new(StringComparer.OrdinalIgnoreCase);

    public CertificateSignInService(IClientCertificateRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    /// <summary>
    /// <inheritdoc cref="ICertificateSignInService.CreatePrincipalForCertificateAsync" />
    /// </summary>
    /// <param name="cert">The client certificate to create a principal for.</param>
    /// <returns>A <see cref="ClaimsPrincipal"/> when the certificate could be converted; otherwise <c>null</c>.</returns>
    public async Task<ClaimsPrincipal?> CreatePrincipalForCertificateAsync(X509Certificate2 cert)
    {
        if (cert == null) return null;

        try
        {
            string subject = cert.Subject ?? string.Empty;
            await EnsureCertificatePersistedAsync(cert, subject);
            List<Claim> claims = BuildClaims(cert);
            ClaimsPrincipal principal = CreatePrincipal(claims);
            return principal;
        }
        catch
        {
            return null;
        }
    }

    // Acquires a per-subject lock and ensures the certificate is persisted in the repository.
    private async Task EnsureCertificatePersistedAsync(X509Certificate2 cert, string subject)
    {
        SemaphoreSlim sem = GetOrCreateSubjectLock(subject);
        // Add timeout to detect deadlocks
        if (!await sem.WaitAsync(TimeSpan.FromSeconds(10)))
            throw new TimeoutException($"Timeout waiting for subject lock: {subject}");
        try
        {
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Acquired lock for subject: {subject}");
            if (!await CertificateExistsAsync(subject))
            {
                ClientCertificate entity = CreateClientCertificate(cert, subject);
                await _repo.AddAsync(entity);
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Added certificate for subject: {subject}");
            }
        }
        finally
        {
            _ = sem.Release();
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Released lock for subject: {subject}");
        }
    }

    private static SemaphoreSlim GetOrCreateSubjectLock(string subject)
    {
        return _subjectLocks.GetOrAdd(subject, _ => new SemaphoreSlim(1, 1));
    }

    private async Task<bool> CertificateExistsAsync(string subject)
    {
        return await _repo.FindBySubjectAsync(subject) != null;
    }

    private static ClientCertificate CreateClientCertificate(X509Certificate2 cert, string subject)
    {
        return new ClientCertificate
        {
            Id = Guid.NewGuid(),
            Subject = subject,
            Issuer = cert.Issuer ?? string.Empty,
            SerialNumber = cert.SerialNumber ?? string.Empty,
            ValidFrom = cert.NotBefore,
            ValidTo = cert.NotAfter
        };
    }

    // Builds claims from the certificate properties.
    private static List<Claim> BuildClaims(X509Certificate2 cert)
    {
        var claims = new List<Claim>
        {
            CreateClaim(ClaimTypes.Name, cert.Subject),
            CreateClaim("thumbprint", cert.Thumbprint),
            CreateClaim("issuer", cert.Issuer),
            CreateClaim("serialNumber", cert.SerialNumber)
        };
        return claims;
    }

    private static Claim CreateClaim(string type, string? value)
    {
        return new Claim(type, value ?? string.Empty);
    }

    // Creates a ClaimsPrincipal from the provided claims.
    private static ClaimsPrincipal CreatePrincipal(List<Claim> claims)
    {
        ClaimsIdentity identity = new(claims, CookieScheme);
        return new ClaimsPrincipal(identity);
    }
}
