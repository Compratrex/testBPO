namespace WholesalePlatform.Domain.Common;

public abstract class AuditableEntity : Entity
{
    protected AuditableEntity()
    {
    }

    protected AuditableEntity(Guid id)
        : base(id)
    {
    }

    public DateTimeOffset CreatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public DateTimeOffset? LastModifiedAt { get; private set; }
    public Guid? LastModifiedBy { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public Guid? DeletedBy { get; private set; }
    public long Version { get; private set; } = 1;

    public void MarkCreated(Guid? userId, DateTimeOffset now)
    {
        CreatedAt = now;
        CreatedBy = userId;
        LastModifiedAt = now;
        LastModifiedBy = userId;
        Version = 1;
    }

    public void MarkModified(Guid? userId, DateTimeOffset now)
    {
        LastModifiedAt = now;
        LastModifiedBy = userId;
        Version++;
    }

    public void MarkDeleted(Guid? userId, DateTimeOffset now)
    {
        IsDeleted = true;
        DeletedAt = now;
        DeletedBy = userId;
        MarkModified(userId, now);
    }
}
