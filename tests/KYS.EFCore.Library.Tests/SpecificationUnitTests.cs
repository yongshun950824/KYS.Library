using System.Linq.Expressions;
using KYS.EFCore.Library.Tests.Domain.Entities;
using NUnit.Framework;

namespace KYS.EFCore.Library.Tests;

public class SpecificationUnitTests
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

    [Test]
    public void IsSatisfiedBy_ReturnsTrueWhenPredicateMatches()
    {
        var spec = new ProductByNameSpecification("Phone");
        var product = new Product { Id = 1, Name = "Phone", Category = new Category { Id = 1, Name = "Electronics" } };

        var satisfied = spec.IsSatisfiedBy(product);

        Assert.IsTrue(satisfied);
    }

    [Test]
    public void IsSatisfiedBy_ReturnsFalseWhenPredicateDoesNotMatch()
    {
        var spec = new ProductByNameSpecification("Phone");
        var product = new Product { Id = 2, Name = "Book", Category = new Category { Id = 2, Name = "Books" } };

        var satisfied = spec.IsSatisfiedBy(product);

        Assert.IsFalse(satisfied);
    }

    [Test]
    public void IsSatisfiedBy_WithAndSpecification_ComposesPredicates()
    {
        var nameSpec = new ProductByNameSpecification("Phone");
        var categorySpec = new ProductByCategoryNameSpecification("Electronics");

        var andSpec = nameSpec.And(categorySpec);

        var product = new Product { Id = 1, Name = "Phone", Category = new Category { Id = 1, Name = "Electronics" } };

        Assert.IsTrue(andSpec.IsSatisfiedBy(product));
    }

    [Test]
    public void IsSatisfiedBy_WithOrSpecification_ComposesPredicates()
    {
        var nameSpec1 = new ProductByNameSpecification("Phone");
        var nameSpec2 = new ProductByNameSpecification("Book");

        var orSpec = nameSpec1.Or(nameSpec2);

        var product = new Product { Id = 2, Name = "Book", Category = new Category { Id = 2, Name = "Books" } };

        Assert.IsTrue(orSpec.IsSatisfiedBy(product));
    }
}
