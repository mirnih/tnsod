using RateLimiter.Writer.Domain.Configuration;
using RateLimiter.Writer.Domain.Interfaces;
using RateLimiter.Writer.Infrastructure.Repositories;
using RateLimiter.Writer.Application.Services;
using RateLimiter.Writer.Grpc.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbOptions>(builder.Configuration.GetSection(MongoDbOptions.SectionName));
builder.Services.AddSingleton<IRateLimitRepository, RateLimitRepository>();
builder.Services.AddSingleton<IRateLimitService, RateLimitService>(); 

builder.Services.AddGrpc();
var app = builder.Build();
app.MapGrpcService<GrpcWriterImpl>();
app.Run();