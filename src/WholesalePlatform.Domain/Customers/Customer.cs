using WholesalePlatform.Domain.Common;

namespace WholesalePlatform.Domain.Customers;

public sealed class Customer : AuditableEntity
{
    public const int MaxNameLength = 200;
    public const int MaxLegalAddressLength = 500;

    private readonly List<CustomerUser> _users = [];

    private Customer()
    {
    }

    private Customer(Guid id, string name, string legalAddress)
        : base(id)
    {
        Name = name;
        LegalAddress = legalAddress;
    }

    public string Name { get; private set; } = string.Empty;
    public string LegalAddress { get; private set; } = string.Empty;
    public IReadOnlyCollection<CustomerUser> Users => _users.AsReadOnly();

    public static Customer Create(string name, string legalAddress, Guid primaryUserId)
    {
        ValidateProfile(name, legalAddress);

        var customer = new Customer(Guid.NewGuid(), name.Trim(), legalAddress.Trim());
        customer.AddUser(primaryUserId, isPrimaryContact: true);

        return customer;
    }

    public void UpdateProfile(string name, string legalAddress)
    {
        ValidateProfile(name, legalAddress);

        Name = name.Trim();
        LegalAddress = legalAddress.Trim();
    }

    public void AddUser(Guid userId, bool isPrimaryContact)
    {
        if (userId == Guid.Empty)
        {
            throw new DomainException("Customer user id cannot be empty.");
        }

        if (_users.Any(customerUser => customerUser.UserId == userId))
        {
            throw new DomainException("User is already linked to this customer.");
        }

        if (isPrimaryContact && _users.Any(customerUser => customerUser.IsPrimaryContact))
        {
            throw new DomainException("Customer already has a primary contact.");
        }

        _users.Add(CustomerUser.Create(Id, userId, isPrimaryContact));
    }

    private static void ValidateProfile(string name, string legalAddress)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Customer name cannot be empty.");
        }

        if (name.Trim().Length > MaxNameLength)
        {
            throw new DomainException($"Customer name cannot exceed {MaxNameLength} characters.");
        }

        if (string.IsNullOrWhiteSpace(legalAddress))
        {
            throw new DomainException("Legal address cannot be empty.");
        }

        if (legalAddress.Trim().Length > MaxLegalAddressLength)
        {
            throw new DomainException($"Legal address cannot exceed {MaxLegalAddressLength} characters.");
        }
    }
}
