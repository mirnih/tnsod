using ConsoleApp1.Domain.Interfaces;

namespace ConsoleApp1.Protos;

public partial class UpdateUserRequest : IUser
{
    string IUser.Login => string.Empty;
}
