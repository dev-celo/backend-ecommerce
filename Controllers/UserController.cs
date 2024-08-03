using Microsoft.AspNetCore.Mvc;
using backend_ecommerce.Models;
using Auth.Dto;
using backend_ecommerce.Services;

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
    public IActionResult GetListUser ()
    {
        var res = _repository.GetListUser();
        return Ok(res);
    }

    [HttpGet("{id}")]
    public IActionResult GetUserById(int id)
    {
        var res = _repository.GetUserById(id);
        if (res == null) {
            return NotFound();
        }
        return Ok(res);
    }

    [HttpPost("signup")]
    public IActionResult AddUser([FromBody] User user)
    {
        _repository.AddUser(user);
        var token = _tokenGenerator.Generate(user);
        return Created("", new { token });
    }

    [HttpPut("{id}")]
    public IActionResult UpdateUser(int id, User updatedUser)
    {
        var res = _repository.UpdateUser(id, updatedUser);
        if (res == null) {
            return NotFound();
        }
        return Ok(res);
    }

    [HttpPost("{login}")]
    public IActionResult Login(LoginDTORequest loginDTO)
    {
        User? existingUser = _repository.GetUserByEmail(loginDTO.Email);

        if (existingUser == null) return Unauthorized(new { message = "Credenciais de Login incorretas" });
        if (existingUser.PasswordHash != loginDTO.Password) return Unauthorized(new { message = "Credenciais de Login incorretas" });

        var token = _tokenGenerator.Generate(existingUser);
        return Ok(new { token });
    }
}