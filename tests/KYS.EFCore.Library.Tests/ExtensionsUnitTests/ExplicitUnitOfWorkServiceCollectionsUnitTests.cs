using KYS.EFCore.Library.Extensions;
using KYS.EFCore.Library.Tests.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace KYS.EFCore.Library.Tests.ExtensionsUnitTests;

public class ExplicitUnitOfWorkServiceCollectionsUnitTests
{
    [Test]
    public void AddExplicitUnitOfWork_RegisteringWithDbContext_ShouldResolveUnitOfWork()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDbContext<TestDbContext>(options => options.UseSqlite("DataSource=:memory:"));
        services.AddExplicitUnitOfWork<TestDbContext>();

        var serviceProvider = services.BuildServiceProvider();

        // Act
        var unitOfWork = serviceProvider.GetService<IUnitOfWork>();

        // Assert
        Assert.IsNotNull(unitOfWork);
    }

    [Test]
    public void AddExplicitUnitOfWork_ShouldReturnSameServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddExplicitUnitOfWork<TestDbContext>();

        // Assert
        Assert.AreSame(services, result);
    }

    [Test]
    public void AddExplicitUnitOfWork_ShouldCreateScopedUnitOfWorkPerScope()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDbContext<TestDbContext>(options => options.UseSqlite("DataSource=:memory:"));
        services.AddExplicitUnitOfWork<TestDbContext>();

        var serviceProvider = services.BuildServiceProvider();

        // Act
        using var scope1 = serviceProvider.CreateScope();
        var unitOfWork1 = scope1.ServiceProvider.GetService<IUnitOfWork>();

        using var scope2 = serviceProvider.CreateScope();
        var unitOfWork2 = scope2.ServiceProvider.GetService<IUnitOfWork>();

        // Assert
        Assert.IsNotNull(unitOfWork1);
        Assert.IsNotNull(unitOfWork2);
        // Assert that different scopes get different instances
        Assert.AreNotSame(unitOfWork1, unitOfWork2);
    }

    [Test]
    public void AddExplicitUnitOfWork_ShouldResolveConcreteUnitOfWorkType()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDbContext<TestDbContext>(options => options.UseSqlite("DataSource=:memory:"));
        services.AddExplicitUnitOfWork<TestDbContext>();

        var serviceProvider = services.BuildServiceProvider();

        // Act
        var unitOfWork = serviceProvider.GetService<IUnitOfWork>();

        // Assert
        Assert.IsNotNull(unitOfWork);
        Assert.IsInstanceOf<DbContextUnitOfWork<TestDbContext>>(unitOfWork);
    }

    [Test]
    public void AddExplicitUnitOfWork_WithSameScope_ShouldReturnSameInstance()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDbContext<TestDbContext>(options => options.UseSqlite("DataSource=:memory:"));
        services.AddExplicitUnitOfWork<TestDbContext>();

        var serviceProvider = services.BuildServiceProvider();

        // Act
        using var scope = serviceProvider.CreateScope();
        var unitOfWork1 = scope.ServiceProvider.GetService<IUnitOfWork>();
        var unitOfWork2 = scope.ServiceProvider.GetService<IUnitOfWork>();

        // Assert
        Assert.IsNotNull(unitOfWork1);
        Assert.IsNotNull(unitOfWork2);
        // Assert that the same scope gets the same instance
        Assert.AreSame(unitOfWork1, unitOfWork2);
    }

    [Test]
    public void AddExplicitUnitOfWork_ShouldUseSameScopeDbContextInstance()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDbContext<TestDbContext>(options => options.UseSqlite("DataSource=:memory:"));
        services.AddExplicitUnitOfWork<TestDbContext>();

        var serviceProvider = services.BuildServiceProvider();

        // Act
        using var scope = serviceProvider.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
        var dbContext = scope.ServiceProvider.GetService<TestDbContext>();

        var field = unitOfWork!.GetType().GetField("_context", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var unitOfWorkDbContext = field!.GetValue(unitOfWork) as TestDbContext;

        // Assert
        Assert.IsNotNull(unitOfWork);
        Assert.IsNotNull(dbContext);
        Assert.IsNotNull(unitOfWorkDbContext);
        // Assert that the same scope gets the same DbContext instance
        Assert.AreSame(dbContext, unitOfWorkDbContext);
    }
}
