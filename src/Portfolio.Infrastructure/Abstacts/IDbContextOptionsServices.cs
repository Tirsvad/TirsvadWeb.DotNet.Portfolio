using Microsoft.EntityFrameworkCore;

using Portfolio.Infrastructure.Persistences;

namespace Portfolio.Infrastructure.Abstacts;

public interface IDbContextOptionsServices
{
    DbContextOptions<ApplicationDbContext> CreateOptions(string environment, string? name = null);
    string CreateConnectionString(string environment, string? name = null);
}