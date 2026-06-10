namespace WholesalePlatform.WebApi.Contracts.Common;

public sealed record ApiResponse<T>(bool Succeeded, T Data)
{
    public static ApiResponse<T> Success(T data)
    {
        return new ApiResponse<T>(true, data);
    }
}
