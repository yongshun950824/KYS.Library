using KYS.EFCore.Library.Tests.Domain.Entities;
using KYS.EFCore.Library.Tests.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace KYS.EFCore.Library.Tests;

public class DbContextUnitOfWorkUnitTests
{
    private TestDbContext? _context;

    [SetUp]
    public void Setup()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(connection)
            .Options;

        _context = new TestDbContext(options);
        _context.Database.EnsureCreated();
    }

    [Test]
    public async Task BeginAsync_ShouldStartTransaction()
    {
        // Arrange
        using var unitOfWork = new DbContextUnitOfWork<TestDbContext>(_context!);

        // Act
        await unitOfWork.BeginAsync();

        // Assert
        Assert.IsNotNull(_context!.Database.CurrentTransaction);
    }

    [Test]
    public async Task CommitAsync_WithoutBeginAsyncCalled_ShouldThrowInvalidOperationException()
    {
        // Arrange
        using var unitOfWork = new DbContextUnitOfWork<TestDbContext>(_context!);

        // Act
        var exception = Assert.ThrowsAsync<InvalidOperationException>(async () => await unitOfWork.CommitAsync());

        // Assert
        Assert.AreEqual($"{nameof(DbContextUnitOfWork<TestDbContext>.BeginAsync)}() must be called first.", exception?.Message);
    }

    [Test]
    public async Task CommitAsync_WithBeginAsyncCalled_ShouldCommitTransaction()
    {
        // Arrange
        await using var unitOfWork = new DbContextUnitOfWork<TestDbContext>(_context!);

        // Act
        await unitOfWork.BeginAsync();

        var category = new Category { Name = "Test Category" };
        _context!.Categories.Add(category);

        var product = new Product { Name = "Test Product", Category = category };
        _context!.Products.Add(product);

        await unitOfWork.CommitAsync();

        // Assert
        Assert.AreEqual(1, _context!.Products.Count());
        Assert.AreEqual(1, _context!.Categories.Count());
        Assert.IsTrue(_context!.Products.Any(p => p.Name == product.Name));
        Assert.IsTrue(_context!.Categories.Any(c => c.Name == category.Name));
    }

    [Test]
    public async Task RollbackAsync_WithoutBeginAsyncCalled_ShouldThrowInvalidOperationException()
    {
        // Arrange
        using var unitOfWork = new DbContextUnitOfWork<TestDbContext>(_context!);

        // Act
        var exception = Assert.ThrowsAsync<InvalidOperationException>(async () => await unitOfWork.RollbackAsync());

        // Assert
        Assert.AreEqual($"{nameof(DbContextUnitOfWork<TestDbContext>.BeginAsync)}() must be called first.", exception?.Message);
    }

    [Test]
    public async Task RollbackAsync_WithBeginAsyncCalled_ShouldRollbackTransaction()
    {
        // Arrange
        await using var unitOfWork = new DbContextUnitOfWork<TestDbContext>(_context!);

        // Act
        await unitOfWork.BeginAsync();

        var category = new Category { Name = "Test Category" };
        _context!.Categories.Add(category);
        await _context!.SaveChangesAsync();

        var product = new Product { Name = "Test Product", Category = category };
        _context!.Products.Add(product);

        await unitOfWork.RollbackAsync();

        // Assert
        Assert.AreEqual(0, _context!.Products.Count());
        Assert.AreEqual(0, _context!.Categories.Count());
        Assert.IsFalse(_context!.Products.Any(p => p.Name == product.Name));
        Assert.IsFalse(_context!.Categories.Any(c => c.Name == category.Name));
    }

    [TearDown]
    public void TearDown()
    {
        _context?.Dispose();
    }
}
