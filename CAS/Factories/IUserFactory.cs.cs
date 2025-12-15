using CAS.Models.Users;
namespace CAS.Factories
{
    public interface IUserFactory
    {
        User CreateUser(string userType);
    }

}
