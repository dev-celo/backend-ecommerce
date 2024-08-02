using backend_ecommerce.Models;

public class UserRepository
{
    private readonly EcommerceContext _context;

    public UserRepository(EcommerceContext context)
    {
        _context = context;
    }

    public List<User?> GetListUser()
    {
        var query = _context.Users.ToList();
        return query;
    }

    public User GetUser(int id)
    {
        try {
            return _context.Users.Find(id);
        }
        catch (Exception ex) {
            BadRequestResult(ex.Message);
            return null;
        }
    }

    public User AddUser(User user)
    {
        _context.Add(user);
        _context.SaveChanges();
        return user;
    }

    public User UpdateUser(int id, User updatedUser)
    {
        var user = _context.Users.Find(id);

        if (user != null)
        {
            user.UserName = updatedUser.UserName;
            user.Email = updatedUser.Email;

            _context.SaveChanges();
        }
        return user;
    }

    private void BadRequestResult(string message)
    {
        throw new NotImplementedException();
    }
}