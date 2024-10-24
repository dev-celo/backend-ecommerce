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
        try
        {
            return _context.Users.Find(id);
        }
        catch (Exception ex)
        {
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

    public bool ChangePassword(int id, string currentPassword, string newPassword)
    {
        var user = GetUserById(id);

        if (user == null)
        {
            return false; // Usuário não encontrado
        }

        // Verifica se a senha atual fornecida corresponde ao hash armazenado
        string storedSalt = user.Salt;
        string storedHash = user.PasswordHash;
        var passwordIsEquals = CustomPasswordHasher.VerifyPassword(currentPassword, storedSalt, storedHash);
        if (!passwordIsEquals)
        {
            return false; // Senha atual incorreta
        }

        // Gera um novo hash para a nova senha
        user.PasswordHash = CustomPasswordHasher.HashPassword(newPassword, out string newSalt);
        user.Salt = newSalt; // Atualize o salt no banco de dados

        _context.Update(user);
        _context.SaveChanges();

        return true; // Senha alterada com sucesso
    }


    public bool UpdatePassword(string email, string newPassword)
    {
        // Localiza o usuário pelo email
        var user = _context.Users.SingleOrDefault(u => u.Email == email);
        if (user == null)
        {
            return false; // Usuário não encontrado
        }

        // Gera um novo hash e salt para a nova senha
        user.PasswordHash = CustomPasswordHasher.HashPassword(newPassword, out string newSalt);
        user.Salt = newSalt; // Atualize o salt no banco de dados

        _context.Entry(user).State = EntityState.Modified;
        _context.SaveChanges();

        return true; // Senha atualizada com sucesso
    }
}