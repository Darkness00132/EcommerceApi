using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.Identity;

public sealed class AppRole : IdentityRole<Guid>
{
    private AppRole() { }
    public AppRole(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
        NormalizedName = name.ToUpperInvariant();
        ConcurrencyStamp = Guid.NewGuid().ToString();
    }

    public AppRole(Guid id, string name)
    {
        Id = id;
        Name = name;
        NormalizedName = name.ToUpperInvariant();
        ConcurrencyStamp = Guid.NewGuid().ToString();
    }
}