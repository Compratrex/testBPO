using WholesalePlatform.Application.Auth.Commands.SetPassword;
using WholesalePlatform.Domain.Users;
using WholesalePlatform.Tests.TestDoubles;

namespace WholesalePlatform.Tests.Application;

public sealed class PasswordSetupTests
{
    [Fact]
    public async Task SetPassword_completes_active_setup_token()
    {
        var passwordHasher = new FakePasswordHasher();
        var now = new FakeDateTimeProvider();
        var user = User.CreateCustomer(
            "customer@example.com",
            "Customer",
            passwordHasher.HashPassword("temporary"));
        user.StartPasswordSetup(passwordHasher.HashPassword("setup-token"), now.UtcNow.AddHours(1));
        var unitOfWork = new FakeUnitOfWork(users: [user]);

        var handler = new SetPasswordCommandHandler(unitOfWork, passwordHasher, now);

        await handler.Handle(
            new SetPasswordCommand("customer@example.com", "setup-token", "NewPassword123"),
            CancellationToken.None);

        Assert.False(user.IsPasswordSetupRequired);
        Assert.True(passwordHasher.VerifyPassword(user.PasswordHash, "NewPassword123"));
        Assert.Equal(1, unitOfWork.SaveChangesCount);
    }
}
