using ConsoleApp1.Domain.Interfaces;

namespace ConsoleApp1.Domain.Entities;

public class User : IUser
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public string Surname { get; set; }
}