using Microsoft.AspNetCore.Mvc;
using backend_ecommerce.Models;
using backend_ecommerce.Services;
using Auth.Dto;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

[ApiController]
[Route("[controller]")]

public class UserController : ControllerBase
{
    private readonly UserRepository _repository;
    private readonly TokenGenerator _tokenGenerator;

    public UserController(UserRepository repository)
    {
        _repository = repository;
        _tokenGenerator = new TokenGenerator();
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = "Admin")]
    public IActionResult GetListUser ()
    {
        var res = _repository.GetListUser();
        return Ok(res);
    }

    [HttpGet("{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = "Admin, User")]
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
    [Authorize(Policy = "User")]
    public IActionResult UpdateUser(int id, User updatedUser)
    {
        var res = _repository.UpdateUser(id, updatedUser);
        if (res == null) {
            return NotFound();
        }
        return Ok(res);
    }

    [HttpPost("{login}")]
    [AllowAnonymous]
    public IActionResult Login(LoginDTORequest loginDTO)
    {
        User? existingUser = _repository.GetUserByEmail(loginDTO.Email);

        if (existingUser == null) return Unauthorized(new { message = "Credenciais de Login incorretas" });
        if (existingUser.PasswordHash != loginDTO.Password) return Unauthorized(new { message = "Credenciais de Login incorretas" });

        var token = _tokenGenerator.Generate(existingUser);
        return Ok(new { token });
    }
    
    [HttpPost("signup")]
    [AllowAnonymous]
    public IActionResult AddUser([FromBody] User user)
    {
        _repository.AddUser(user);
        var token = _tokenGenerator.Generate(user);
        return Created("", new { token });
    }
}