using ConsoleApp1.Domain.Interfaces;

namespace ConsoleApp1.Protos;

public partial class CreateUserRequest : IUser
{
    int IUser.Id => 0;
}
