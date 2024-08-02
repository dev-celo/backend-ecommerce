using Microsoft.AspNetCore.Mvc;
using backend_ecommerce.Models;

[ApiController]
[Route("[controller]")]

public class UserController : ControllerBase
{
    private readonly UserRepository _repository;

    public UserController(UserRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public IActionResult GetListUser ()
    {
        var res = _repository.GetListUser();
        return Ok(res);
    }

    [HttpGet("{id}")]
    public IActionResult GetUser(int id)
    {
        var res = _repository.GetUser(id);
        if (res == null) {
            return NotFound();
        }
        return Ok(res);
    }

    [HttpPost]
    public IActionResult AddUser (User user)
    {
        _repository.AddUser(user);
        return Ok(new { message = "User added"});
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
}