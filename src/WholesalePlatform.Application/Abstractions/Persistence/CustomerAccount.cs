using WholesalePlatform.Domain.Customers;
using WholesalePlatform.Domain.Users;

namespace WholesalePlatform.Application.Abstractions.Persistence;

public sealed record CustomerAccount(Customer Customer, User PrimaryUser);
