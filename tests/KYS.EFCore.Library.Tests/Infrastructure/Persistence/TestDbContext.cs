using KYS.EFCore.Library.Tests.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KYS.EFCore.Library.Tests.Infrastructure.Persistence;

internal class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
}