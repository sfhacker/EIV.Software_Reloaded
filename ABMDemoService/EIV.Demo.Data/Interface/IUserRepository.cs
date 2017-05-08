
namespace EIV.Demo.Data.Interface
{
    using EIV.Demo.Data.Base;
    using Model;

    public interface IUserRepository : IRepository<User>
    {
        User Authenticate(string userName, string userPassword);
        User GetByName(string userName);
    }
}