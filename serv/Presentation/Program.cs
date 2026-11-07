using ConsoleApp1.Application.Services;
using ConsoleApp1.Application.Validators;
using ConsoleApp1.Domain.Configuration;
using ConsoleApp1.Domain.Interfaces;
using ConsoleApp1.Grpc.Services;
using ConsoleApp1.Infrastructure.Repositories;
using ConsoleApp1.Protos;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DbConnectionOptions>(builder.Configuration.GetSection(DbConnectionOptions.SectionName));

builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IUserService, UserService>();

builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
});

builder.Services.AddSingleton<IValidator<CreateUserRequest>, CreateUserValidator>();
builder.Services.AddSingleton<IValidator<UpdateUserRequest>, UpdateUserValidator>();
var app = builder.Build();
app.MapGrpcService<GrpcUserServiceImpl>();

app.Run();