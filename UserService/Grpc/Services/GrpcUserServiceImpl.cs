using ConsoleApp1.Application.Mappers;
using ConsoleApp1.Domain.Interfaces;
using ConsoleApp1.Protos;
using FluentValidation;
using Grpc.Core;

namespace ConsoleApp1.Grpc.Services;

public class GrpcUserServiceImpl : UserServiceGrpc.UserServiceGrpcBase
{
    private readonly IUserService _userService;
    private readonly IValidator<CreateUserRequest> _createUserValidator;
    private readonly IValidator<UpdateUserRequest> _updateUserValidator;

    public GrpcUserServiceImpl(IUserService userService, IValidator<CreateUserRequest> createUserValidator, IValidator<UpdateUserRequest> updateUserValidator)
    {
        _userService = userService;
        _createUserValidator = createUserValidator;
        _updateUserValidator = updateUserValidator;
    }

    public override async Task<CreateUserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
    {
        var validationResult =  _createUserValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("\n", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new RpcException(new Status(StatusCode.InvalidArgument, errorMessages));
        }

        var newUserId = await _userService.CreateUserAsync(request, context.CancellationToken);

        return new CreateUserResponse { Id = newUserId };
    }

    public override async Task<DeleteUserResponse> DeleteUser(DeleteUserRequest request, ServerCallContext context)
    {
        var result = await _userService.DeleteUserAsync(request.Id, context.CancellationToken);
        return new DeleteUserResponse{ Result = result };
    }

    public override async Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request, ServerCallContext context)
    {
        var validationResult =  _updateUserValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("\n", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new RpcException(new Status(StatusCode.InvalidArgument, errorMessages));
        }
        var result = await _userService.UpdateUserAsync(request, context.CancellationToken);
        return new UpdateUserResponse{ Result = result };
    }

    public override async Task<GetUserByIdResponse> GetUserById(GetUserByIdRequest request, ServerCallContext context)
    {
        var user = await _userService.GetUserByIdAsync(request.Id, context.CancellationToken);
        if (user == null)
        {
            return new GetUserByIdResponse();
        }
        
        var userModel = UserMapper.ToUserModel(user);
        return new GetUserByIdResponse{ User = userModel };
    
    }

    public override async Task<GetUsersByNameResponse> GetUsersByName(GetUsersByNameRequest request, ServerCallContext context)
    {
        var users = await _userService.GetByNameAndSurnameAsync(request.Name, request.Surname, context.CancellationToken);
        var response =  new GetUsersByNameResponse();
        response.Users.AddRange(UserMapper.ToUserModels(users));
        return response;
    }
    
}