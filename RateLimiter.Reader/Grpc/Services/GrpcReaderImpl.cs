using Grpc.Core;
using RateLimiter.Reader.Domain.Interfaces;

namespace RateLimiter.Reader.Grpc.Services;

public class GrpcReaderImpl : Reader.ReaderBase
{
    private readonly IReaderService _readerService;

    public GrpcReaderImpl(IReaderService readerService)
    {
        _readerService = readerService;
    }

    public override Task<PingResponse> Ping(PingRequest request, ServerCallContext context)
    {
        return Task.FromResult(new PingResponse { Status = "Alive" });
    }

    public override Task<GetAllLimitsResponse> GetAllLimits(GetAllLimitsRequest request, ServerCallContext context)
    {
        var items = _readerService.GetAllLimits();
        var response = new GetAllLimitsResponse();
        foreach (var x in items)
        {
            response.Limits.Add(new RateLimitModel
            {
                Route = x.Route,
                RequestsPerMinute = x.RequestsPerMinute
            });
        }
        return Task.FromResult(response);
    }
}