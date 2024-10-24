using Microsoft.AspNetCore.Mvc;
using backend_ecommerce.Models;
using backend_ecommerce.Services;
using Auth.Dto;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;

[ApiController]
[Route("[controller]")]

public class UserController : ControllerBase
{
    private readonly UserRepository _repository;
    private readonly TokenGenerator _tokenGenerator;

    public UserController(UserRepository repository, TokenGenerator tokenGenerator)
    {
        _repository = repository;
        _tokenGenerator = tokenGenerator;
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = "User")]
    public IActionResult GetListUser ()
    {
        var res = _repository.GetListUser();
        return Ok(res);
    }

    [HttpGet("{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = "User")]
    public IActionResult GetUserById(int id)
    {
        var res = _repository.GetUserById(id);
        if (res == null) {
            return NotFound();
        }
        return Ok(res);
    }

    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = "Admin")]
    public IActionResult DeleteUser(int id)
    {
        var user = _repository.GetUserById(id);
        if (user == null) return NotFound();

        _repository.DeleteUser(id);
        return Ok(new { message = "User deleted" });
    }

    [HttpPut("{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult UpdateUser(int id, User updatedUser)
    {
        var res = _repository.UpdateUser(id, updatedUser);
        if (res == null) {
            return NotFound();
        }
        return Ok(res);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public IActionResult Login(UserDTO loginDTO)
    {
        User? existingUser = _repository.GetUserByEmail(loginDTO.Email);
        if (existingUser == null || !CustomPasswordHasher.VerifyPassword(loginDTO.Password, existingUser.Salt, existingUser.PasswordHash)) {
            return BadRequest(new { message = "Invalid credentials" });
        }

        var token = _tokenGenerator.Generate(existingUser);
        return Ok(new { token });
    }
    
    [HttpPost("signup")]
    [AllowAnonymous]
    public IActionResult AddUser([FromBody] User user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        //Verificar se o usuário já existe
        var existingUser = _repository.GetUserByEmail(user.Email);

        if(existingUser != null)
        {
            return BadRequest(new { message = "Invalid email"});
        }
        
       // Hash a senha e obter o salt
        string salt;
        user.PasswordHash = CustomPasswordHasher.HashPassword(user.PasswordHash, out salt);
        user.Salt = salt;
        user.Access = "User";

        
        _repository.AddUser(user);

        var token = _tokenGenerator.Generate(user);
        return Created("", new { token });
    }

    // Usado quando o usuário deseja alterar a senha enquanto ainda está logado no sistema.
    [HttpPost("change-password/{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy="User")]
    public IActionResult ChangePassword(int id, ChangePasswordDTO changePasswordDTO)
    {
        Boolean passwordChanged = _repository.ChangePassword(id, changePasswordDTO.CurrentPassword, changePasswordDTO.NewPassword);
        if (passwordChanged == false)
        {
            return BadRequest(new { message = "Senha atual incorreta" });
        }

        return Ok(new { message = "Senha alterada com sucesso" });
    }

    // Usado quando o usuário esqueceu a senha.
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public IActionResult ForgotPassword([FromBody] ForgotPasswordDTO forgotPasswordDTO)
    {
        var user = _repository.GetUserByEmail(forgotPasswordDTO.Email);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        var token = _tokenGenerator.Generate(user);
        // Lógica para enviar email com o token
        
        return Ok(new { token });
    }
    // Geralmente faz parte do processo de forgot password.
    [HttpPost("reset-password")]
    public IActionResult ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
    {
        var claimsPrincipal = _tokenGenerator.ValidateToken(resetPasswordDTO.Token);
        if (claimsPrincipal == null)
        {
            return Unauthorized(new { message = "Invalid or expired token" });
        }

        var userId = int.Parse(claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier).Value);
        var user = _repository.GetUserById(userId);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        _repository.UpdatePassword(user.Email, resetPasswordDTO.NewPassword);
        return Ok(new { message = "Password has been reset successfully." });
    }
}