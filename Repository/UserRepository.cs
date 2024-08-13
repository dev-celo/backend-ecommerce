using backend_ecommerce.Models;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;


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

    public User GetUserById(int id)
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
        user.Access = "User";
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

    public void DeleteUser(int id)
    {
        var user = _context.Users.Find(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
        }
    }

    public User? GetUserByEmail(string email)
    {
        User? existingUser = _context.Users.Where(u => u.Email == email).FirstOrDefault();
        return existingUser;
    }
    
    private void BadRequestResult(string message)
    {
        throw new NotImplementedException();
    }

    public bool ChangePassword(int id, string CurrentPassword, string newPassword)
    {
        var UserFinded = GetUserById(id);

        if (UserFinded == null || UserFinded.PasswordHash != newPassword)
        {
            return false;
        }

        UserFinded.PasswordHash = newPassword;
        _context.Update(UserFinded);
        _context.SaveChanges();

        return true;
    }

    public bool UpdatePassword(string email, string newPassword)
    {
        // Localiza o usuário pelo email
        var user = _context.Users.SingleOrDefault(u => u.Email == email);
        if (user == null)
        {
            return false; // Usuário não encontrado
        }

        // Gera um novo hash de senha
        user.PasswordHash = HashPassword(newPassword);

        // Marca o usuário como modificado
        _context.Entry(user).State = EntityState.Modified;

        // Salva as alterações no banco de dados
        _context.SaveChanges();
        return true;
    }

    private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                // Gera o hash da senha
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Converte o hash em uma string
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
 }