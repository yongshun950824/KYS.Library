using KYS.EFCore.Library.DBContext;
using KYS.EFCore.Library.Tests.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KYS.EFCore.Library.Tests.Infrastructure.Persistence;

internal class TestApplicationDbContext : ApplicationDbContext
{
    public TestApplicationDbContext(DbContextOptions<TestApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure a non-primary, database-generated column so EF treats it
        // as temporary until after SaveChanges. SQLite supports randomblob(),
        // hex() and lower(), which we use to produce a short generated string.
        modelBuilder.Entity<Category>()
            .Property(c => c.GeneratedCode)
            .HasColumnName("GeneratedCode")
            .HasDefaultValueSql("lower(hex(randomblob(4)))")
            .ValueGeneratedOnAdd();
    }
}
