using RateLimiter.Reader.Domain.Configuration;
using RateLimiter.Reader.Application.Services;
using RateLimiter.Reader.Application.Mapping;
using RateLimiter.Reader.Domain.Interfaces;
using RateLimiter.Reader.Grpc.Services;
using RateLimiter.Reader.Hosted;
using RateLimiter.Reader.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbOptions>(builder.Configuration.GetSection(MongoDbOptions.SectionName));
builder.Services.AddSingleton<IRateLimitMapper, RateLimitMapper>();
builder.Services.AddSingleton<IMapperFactory, MapperFactory>();
builder.Services.AddSingleton<IRateLimitRepository, RateLimitRepository>();
builder.Services.AddSingleton<IReaderService, ReaderService>();
builder.Services.AddHostedService<ReaderHostedService>();
builder.Services.AddGrpc();

var app = builder.Build();
app.MapGrpcService<GrpcReaderImpl>();
app.Run();