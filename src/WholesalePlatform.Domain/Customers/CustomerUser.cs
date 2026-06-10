using WholesalePlatform.Domain.Common;
using WholesalePlatform.Domain.Users;

namespace WholesalePlatform.Domain.Customers;

public sealed class CustomerUser : AuditableEntity
{
    private CustomerUser()
    {
    }

    private CustomerUser(Guid id, Guid customerId, Guid userId, bool isPrimaryContact)
        : base(id)
    {
        CustomerId = customerId;
        UserId = userId;
        IsPrimaryContact = isPrimaryContact;
    }

    public Guid CustomerId { get; private set; }
    public Guid UserId { get; private set; }
    public bool IsPrimaryContact { get; private set; }
    public User? User { get; private set; }

    public static CustomerUser Create(Guid customerId, Guid userId, bool isPrimaryContact)
    {
        if (customerId == Guid.Empty)
        {
            throw new DomainException("Customer id cannot be empty.");
        }

        if (userId == Guid.Empty)
        {
            throw new DomainException("User id cannot be empty.");
        }

        return new CustomerUser(Guid.NewGuid(), customerId, userId, isPrimaryContact);
    }
}
