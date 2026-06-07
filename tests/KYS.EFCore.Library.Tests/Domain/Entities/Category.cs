namespace KYS.EFCore.Library.Tests.Domain.Entities;

internal class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    // Database-generated non-primary property used in tests to exercise
    // temporary properties handling in ApplicationDbContext auditing.
    public string? GeneratedCode { get; set; }
}
