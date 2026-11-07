using Grpc.Core;
using Grpc.Core.Interceptors;

namespace ConsoleApp1.Grpc.Interceptors;

public sealed class AuthenticationInterceptor : Interceptor
{
    private const string UserIdHeader = "user_id";

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var userId = context.RequestHeaders.GetValue(UserIdHeader);

        if (string.IsNullOrEmpty(userId))
        {
            throw new RpcException(new Status(
                StatusCode.Unauthenticated,
                "Authentication required. Please provide 'user_id' header."));
        }

        context.UserState["user_id"] = userId;

        return await continuation(request, context);
    }
}
