namespace KYS.EFCore.Library.Tests;

public class TestEntity : Entity<long>
{
    public void SetId(long id)
    {
        Id = id;
    }
}
