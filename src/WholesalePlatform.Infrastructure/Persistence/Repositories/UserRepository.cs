using Microsoft.EntityFrameworkCore;
using WholesalePlatform.Application.Abstractions.Persistence;
using WholesalePlatform.Domain.Enums;
using WholesalePlatform.Domain.Users;

namespace WholesalePlatform.Infrastructure.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<User?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken,
        bool includePermissions = false)
    {
        return ApplyPermissions(_context.Users, includePermissions)
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken,
        bool includePermissions = false)
    {
        var normalizedEmail = User.NormalizeEmail(email);

        return ApplyPermissions(_context.Users, includePermissions)
            .FirstOrDefaultAsync(user => user.Email == normalizedEmail, cancellationToken);
    }

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
    {
        var normalizedEmail = User.NormalizeEmail(email);

        return _context.Users.AnyAsync(user => user.Email == normalizedEmail, cancellationToken);
    }

    public Task AddAsync(User user, CancellationToken cancellationToken)
    {
        return _context.Users.AddAsync(user, cancellationToken).AsTask();
    }

    public void Remove(User user)
    {
        _context.Users.Remove(user);
    }

    private static IQueryable<User> ApplyPermissions(IQueryable<User> query, bool includePermissions)
    {
        return includePermissions ? query.Include(user => user.Permissions) : query;
    }
}
