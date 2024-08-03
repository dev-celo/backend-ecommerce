namespace Auth.Dto;

public class LoginDTORequest
{
    public string? UserName { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
}

public class LoginDTOResponse
{
    public string? Token { get; set; }
}