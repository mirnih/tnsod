using Grpc.Core;
using RateLimiter.Writer.Application.Mappers;
using RateLimiter.Writer.Domain.Interfaces;
namespace RateLimiter.Writer.Grpc.Services;

public class GrpcWriterImpl : Writer.WriterBase
{
    private readonly IRateLimitService _rateLimitService;

    public GrpcWriterImpl(IRateLimitService rateLimitService)
    {
        _rateLimitService = rateLimitService;
    }

    public override async Task<CreateLimitResponse> CreateLimit(CreateLimitRequest request, ServerCallContext context)
    {
        var domainLimit = RateLimitMapper.ToRateLimit(request);
        var success = await _rateLimitService.CreateLimitAsync(domainLimit, context.CancellationToken);

        if (!success)
        {
            throw new RpcException(new Status(StatusCode.AlreadyExists, $"Лимит для route '{request.Route}' уже занят"));
        }
        
        return new CreateLimitResponse { Success = true };
    }

    public override async Task<GetLimitByRouteResponse> GetLimitByRoute(GetLimitByRouteRequest request, ServerCallContext context)
    {
        var domainLimit = await _rateLimitService.GetLimitByRouteAsync(request.Route, context.CancellationToken);
        var response = new GetLimitByRouteResponse();
        if (domainLimit != null)
        {
            response.Limit = RateLimitMapper.ToRateLimitModel(domainLimit);
        }
        return response;
    }
    
    public override async Task<UpdateLimitResponse> UpdateLimit(UpdateLimitRequest request, ServerCallContext context)
    {
        var domainLimit = RateLimitMapper.ToRateLimit(request);
        var success = await _rateLimitService.UpdateLimitAsync(domainLimit, context.CancellationToken);
        if (!success)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Лимит для route '{request.Route}' не найден"));
        }
        return new UpdateLimitResponse { Success = success };
    }
    
    public override async Task<DeleteLimitResponse> DeleteLimit(DeleteLimitRequest request, ServerCallContext context)
    {
        var success = await _rateLimitService.DeleteLimitAsync(request.Route, context.CancellationToken);
        if (!success)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Лимит для route '{request.Route}' не найден"));
        }
        return new DeleteLimitResponse { Success = success };
    }
}