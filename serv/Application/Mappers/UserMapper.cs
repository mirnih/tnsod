using ConsoleApp1.Domain.Entities;
using ConsoleApp1.Protos;
using Riok.Mapperly.Abstractions;

namespace ConsoleApp1.Application.Mappers;
[Mapper]
public static partial class UserMapper
{
    public static partial UserModel ToUserModel(User user);
    
    public static partial IEnumerable<UserModel> ToUserModels(IEnumerable<User> users);
    
    public static partial User ToUser(CreateUserRequest request);

    public static partial User ToUser(UpdateUserRequest request);
}