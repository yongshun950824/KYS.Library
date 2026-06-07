namespace KYS.EFCore.Library.Tests;

public class OtherEntity : Entity<long>
{
    public void SetId(long id)
    {
        Id = id;
    }
}
