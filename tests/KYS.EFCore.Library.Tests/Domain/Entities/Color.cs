namespace KYS.EFCore.Library.Tests.Domain.Entities;

internal class Color : Enumeration
{
    public static readonly Color Red = new(1, "Red");
    public static readonly Color Green = new(2, "Green");
    public static readonly Color Blue = new(3, "Blue");

    private Color(long id, string name)
        : base(id, name)
    {
    }
}