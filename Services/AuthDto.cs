namespace Auth.Dto;

public class UserDTO
{
    public string? UserName { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
}

public class LoginDTOResponse
{
    public string? Token { get; set; }
}

public class ChangePasswordDTO
{
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
}

public class ForgotPasswordDTO
{
    public string Email { get; set; }
}

public class ResetPasswordDTO
{
    public string Token { get; set; }
    public string NewPassword { get; set; }
}
