using System.Collections.Concurrent;

namespace copilot_test.Data
{
    /// <summary>
    /// This repository is expected to be a singleton, and only used in development as an example.
    /// </summary>
    /// <param name="logger"></param>
    public class UserRepository
        (ILogger<UserRepository> logger)
        : IUserRepository
    {
        private readonly ConcurrentDictionary<Guid, User> _users = new ();

        public User CreateUser(User user)
        {
            _users[user.Id] = user;
            logger.LogInformation("User created: {userId}", user.Id);
            return user;
        }

        public void DeleteUser(Guid id)
        {
            logger.LogInformation("User deleted: {userId}", id);
            _users.TryRemove(id, out _);
        }

        
        public User? GetUserById(Guid id)
        {
            return _users.TryGetValue(id, out var user) ? user : null;
        }

        public User? UpdateUser(User user)
        {
            if (_users.ContainsKey(user.Id))
            {
                logger.LogInformation("User updated: {userId}", user.Id);
                _users[user.Id] = user;
                return user;
            }
            else
            {
                return null;
            }
        }

        public IReadOnlyCollection<User> GetUsers()
        {
            return _users.Values.ToList().AsReadOnly();
        }
    }
}
