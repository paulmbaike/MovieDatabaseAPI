using MovieDatabaseAPI.Core.Entities;

namespace MovieDatabaseAPI.Core.Interfaces.Repositories;
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
}