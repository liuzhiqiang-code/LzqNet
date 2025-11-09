using System.ComponentModel.DataAnnotations;

namespace LzqNet.Auth.Models;

public class RegisterViewModel
{
    [Required]
    public string UserName { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }

}
