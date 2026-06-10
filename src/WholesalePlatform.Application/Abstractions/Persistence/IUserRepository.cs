using WholesalePlatform.Domain.Users;

namespace WholesalePlatform.Application.Abstractions.Persistence;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken, bool includePermissions = false);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken, bool includePermissions = false);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken);
    Task AddAsync(User user, CancellationToken cancellationToken);
    void Remove(User user);
}
