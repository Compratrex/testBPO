using MediatR;

namespace WholesalePlatform.Application.Customers.Commands.DeleteCustomer;

public sealed record DeleteCustomerCommand(Guid CustomerId) : IRequest;

