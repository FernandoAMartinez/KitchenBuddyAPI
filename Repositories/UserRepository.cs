using KitchenBuddyAPI.Models;
using KitchenBuddyAPI.Services;
using Microsoft.Data.Sqlite;

namespace KitchenBuddyAPI.Repositories;
public interface ISqlLiteRepository<T> where T : class, new()
{
    IEnumerable<T> GetAll();
    T GetById(int id);
    T Create(T value);
    int Update(T value);
    int Delete(int id);
}

public class UserRepository : ISqlLiteRepository<User>
{
    private ISqliteProvider _provider;
    public UserRepository(ISqliteProvider provider)
    {
        _provider = provider;
    }
    public User Create(User value)
    {
        throw new NotImplementedException();
    }

    public int Delete(int id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<User> GetAll()
    {
        var select = "SELECT * FROM Users";

        Console.WriteLine(select);

        var users = _provider.Read(select);

        Console.WriteLine($"Object after select: {users}");

        var list = new List<User>();

        foreach (var u in users)
        {
            var userString = u.ToString().Split('|');
            Console.WriteLine($"Before Split: {u.ToString()}");
            Console.WriteLine($"After Split: {userString}");

            var user = new User()
            {
                Id = Int32.Parse(userString[0]),
                Email = userString[1],
                Password = userString[2],
                Birthday = DateTime.Parse(userString[3]),
                FollowerCount = Int32.Parse(userString[4]),
                FollowingCount = Int32.Parse(userString[5])
            };
            
            list.Add(user);
        }

        return list;
    }

    public User GetById(int id)
    {
        throw new NotImplementedException();
    }

    public int Update(User value)
    {
        throw new NotImplementedException();
    }
}