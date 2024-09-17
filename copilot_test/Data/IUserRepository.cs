using System.Collections.Generic;

namespace copilot_test.Data
{

    public interface IUserRepository
    {
        IReadOnlyCollection<User> GetUsers();
        User? GetUserById(Guid id);
        User CreateUser(User user);
        User? UpdateUser(User user);
        void DeleteUser(Guid id);
    }
}
