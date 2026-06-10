namespace WholesalePlatform.WebApi.Contracts.Auth;

public sealed record SetPasswordRequest(
    string Email,
    string Token,
    string NewPassword);
