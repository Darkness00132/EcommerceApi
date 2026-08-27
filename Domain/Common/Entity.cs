namespace Domain.Common;

public abstract class Entity : IEntity
{
    public Guid Id { get; protected set; }

    protected Entity()
    {
    }

    protected Entity(Guid id)
    {
        Id = id;
    }
}
