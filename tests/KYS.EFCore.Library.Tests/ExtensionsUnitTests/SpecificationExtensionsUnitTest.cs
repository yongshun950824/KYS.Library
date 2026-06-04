using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using KYS.EFCore.Library.Extensions;
using KYS.EFCore.Library.Tests.Domain.Entities;
using KYS.EFCore.Library.Tests.Infrastructure.Persistence;

namespace KYS.EFCore.Library.Tests.ExtensionsUnitTests;

public class SpecificationExtensionsUnitTest
{
    #region Specification classes
    private sealed class ProductByNameSpecification : Specification<Product>
    {
        private readonly string _name;

        public ProductByNameSpecification(string name)
        {
            _name = name;
            ApplyAsNoTracking();
            AddInclude(p => p.Category);
        }

        public override Expression<Func<Product, bool>> ToExpression()
            => p => p.Name == _name;
    }

    private sealed class ProductByCategoryNameSpecification : Specification<Product>
    {
        private readonly string _categoryName;

        public ProductByCategoryNameSpecification(string categoryName)
        {
            _categoryName = categoryName;
            ApplyAsNoTracking();
            AddInclude(p => p.Category);
        }

        public override Expression<Func<Product, bool>> ToExpression()
            => p => p.Category.Name == _categoryName;
    }
    #endregion

    private TestDbContext? _context;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new TestDbContext(options);

        SeedData();
    }

    private void SeedData()
    {
        var electronics = new Category { Id = 1, Name = "Electronics" };
        var books = new Category { Id = 2, Name = "Books" };

        _context!.Categories.AddRange(electronics, books);
        _context!.Products.AddRange(
            new Product { Id = 1, Name = "Phone", Category = electronics },
            new Product { Id = 2, Name = "Book", Category = books }
        );
        _context!.SaveChanges();
    }

    [Test]
    public void ApplySpecification_WithValidSpecification_FiltersAndIncludes()
    {
        var spec = new ProductByNameSpecification("Phone");

        var result = _context.ApplySpecification(spec).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Phone", result[0].Name);
        Assert.IsNotNull(result[0].Category);
        Assert.AreEqual("Electronics", result[0].Category.Name);
    }

    [Test]
    public void ApplySpecification_WithNullSpecification_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _context.ApplySpecification<Product>(null!));
    }

    [Test]
    public void ApplySpecification_WithAndSpecification_ReturnsCorrectResults()
    {
        var productSpec = new ProductByNameSpecification("Phone");
        var categorySpec = new ProductByCategoryNameSpecification("Electronics");

        var spec = productSpec.And(categorySpec);
        var result = _context.ApplySpecification(spec).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Phone", result[0].Name);
        Assert.AreEqual("Electronics", result[0].Category.Name);
    }

    [Test]
    public void ApplySpecification_WithOrSpecification_ReturnsCorrectResults()
    {
        var phoneProductSpec = new ProductByNameSpecification("Phone");
        var bookProductSpec = new ProductByNameSpecification("Book");

        var spec = phoneProductSpec.Or(bookProductSpec);
        var result = _context.ApplySpecification(spec).ToList();

        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.Any(p => p.Name == "Phone"));
        Assert.IsTrue(result.Any(p => p.Name == "Book"));
    }

    [TearDown]
    public void TearDown()
    {
        _context?.Dispose();
    }
}