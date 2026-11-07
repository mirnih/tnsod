using ConsoleApp1.Application.Services;
using ConsoleApp1.Application.Validators;
using ConsoleApp1.Domain.Configuration;
using ConsoleApp1.Domain.Interfaces;
using ConsoleApp1.Grpc.Interceptors;
using ConsoleApp1.Grpc.Services;
using ConsoleApp1.Infrastructure.Repositories;
using ConsoleApp1.Protos;
using FluentValidation;

using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DbConnectionOptions>(builder.Configuration.GetSection(DbConnectionOptions.SectionName));
builder.Services.Configure<RedisOptions>(builder.Configuration.GetSection(RedisOptions.SectionName));

var redisOptions = builder.Configuration.GetSection(RedisOptions.SectionName).Get<RedisOptions>();
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisOptions!.ConnectionString));

builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IUserService, UserService>();

builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
    options.Interceptors.Add<AuthenticationInterceptor>();
    options.Interceptors.Add<RateLimitInterceptor>();
});

builder.Services.AddSingleton<IValidator<CreateUserRequest>, CreateUserValidator>();
builder.Services.AddSingleton<IValidator<UpdateUserRequest>, UpdateUserValidator>();

var app = builder.Build();
app.MapGrpcService<GrpcUserServiceImpl>();

app.Run();