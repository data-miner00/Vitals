namespace Vitals.WebApi.Repositories;

using Vitals.WebApi.Models;

public sealed class UserRepository
{
    private List<User> users = new List<User>();

    public UserRepository()
    {
        users.Add(new User
        {
            Id = 1,
            Email = "test@tesma.com",
            Name = "Test",
        });
    }

    public List<User> GetAll()
    {
        return users;
    }

    public void Add(User user)
    {
        users.Add(user);
    }

    public User Get(int id)
    {
        return users.First(x => x.Id == id);
    }

    public void Update(User user)
    {
        users = [.. users.Where(x => x.Id != user.Id), user];
    }

    public void Delete(int id)
    {
        users = users.Where(x => x.Id != id).ToList();
    }
}
