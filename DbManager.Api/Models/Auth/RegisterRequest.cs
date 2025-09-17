namespace DbManager.Api.Models.Auth;


/// <summary>
/// The request type for the "/register".
/// </summary>
public sealed class RegisterRequest
{
    /// <summary>
    /// User's userName
    /// </summary>
    public required string UserName { get; init; }

    /// <summary>
    /// The user's email address which acts as a user name.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// The user's password.
    /// </summary>
    public required string Password { get; init; }
}
