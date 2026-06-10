namespace WholesalePlatform.WebApi.Routing;

public static class ApiRoutes
{
    private const string Root = "api";

    public static class Auth
    {
        public const string Base = Root + "/auth";
        public const string Login = "login";
        public const string SetPassword = "set-password";
    }

    public static class Customers
    {
        public const string Base = Root + "/customers";
        public const string ById = "{customerId:guid}";

        public static string Location(Guid customerId)
        {
            return $"{Base}/{customerId}";
        }
    }

    public static class Orders
    {
        public const string Base = Root + "/orders";
        public const string My = "my";
        public const string Cancel = "{orderId:guid}/cancel";

        public static string Location(Guid orderId)
        {
            return $"{Base}/{orderId}";
        }
    }

    public static class Products
    {
        public const string Base = Root + "/products";
        public const string ById = "{productId:guid}";

        public static string Location(Guid productId)
        {
            return $"{Base}/{productId}";
        }
    }
}
