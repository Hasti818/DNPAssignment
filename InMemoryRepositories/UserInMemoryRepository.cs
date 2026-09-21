using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class UserInMemoryRepository : IUserRepository
{
    private readonly List<User> users = new();

    public UserInMemoryRepository()
    {
        users.Add(new User
        {
            Id = 1,
            UserName = "Sarah",
            Password = "123"
        });
        users.Add(new User
        {
            Id = 1,
            UserName = "John B",
            Password = "456"
        });
        users.Add(new User
        {
            Id = 1,
            UserName = "JJ",
            Password = "789"
        });
    }
    public Task<User> AddAsync(User user)
    {
        user.Id = users.Any()
            ? users.Max(user => user.Id) + 1
            : 1;

        users.Add(user);

        return Task.FromResult(user);
    }

    public Task UpdateAsync(User user)
    {
        User? existingUser =
            users.SingleOrDefault(existingUser => existingUser.Id == user.Id);

        if (existingUser is null)
        {
            throw new InvalidOperationException(
                $"User with ID '{user.Id}' not found");
        }

        users.Remove(existingUser);
        users.Add(user);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        User? userToRemove =
            users.SingleOrDefault(user => user.Id == id);

        if (userToRemove is null)
        {
            throw new InvalidOperationException(
                $"User with ID '{id}' not found");
        }

        users.Remove(userToRemove);

        return Task.CompletedTask;
    }

    public Task<User> GetSingleAsync(int id)
    {
        User? user =
            users.SingleOrDefault(user => user.Id == id);

        if (user is null)
        {
            throw new InvalidOperationException(
                $"User with ID '{id}' not found");
        }

        return Task.FromResult(user);
    }

    public IQueryable<User> GetMany()
    {
        return users.AsQueryable();
    }
}