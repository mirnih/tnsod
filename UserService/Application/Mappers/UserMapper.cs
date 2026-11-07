using ConsoleApp1.Domain.Interfaces;
using ConsoleApp1.Protos;
using Riok.Mapperly.Abstractions;

namespace ConsoleApp1.Application.Mappers;

[Mapper]
public static partial class UserMapper
{
    public static partial UserModel ToUserModel(IUser user);

    public static partial IEnumerable<UserModel> ToUserModels(IEnumerable<IUser> users);
}