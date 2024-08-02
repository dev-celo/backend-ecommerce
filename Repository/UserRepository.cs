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

    public User AddUser(User user)
    {
        _context.Add(user);
        _context.SaveChanges();
        return user;
    }
}