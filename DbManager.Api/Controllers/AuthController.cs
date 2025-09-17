namespace DbManager.Api.Controllers;

using Microsoft.AspNetCore.Http.HttpResults;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DbManager.Api.Models.Auth;
using DbManager.Api.Services;

/// <summary>
/// Controller responsible for user authentication and registration operations.
/// Provides endpoints for login and user account creation.
/// </summary>
[Route("auth")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    /// <summary>
    /// Authenticates a user based on the provided credentials.
    /// </summary>
    /// <param name="login">The login request containing username/email and password.</param>
    /// <returns>
    /// Returns a <see cref="TokenResponse"/> containing the generated JWT access/refresh tokens 
    /// if authentication is successful.
    /// </returns>
    /// <response code="200">Authentication successful, token is returned.</response>
    /// <response code="400">Invalid credentials or request format.</response>
    [HttpPost("login")]
    public async Task<ActionResult<TokenResponse>> Login(LoginRequest login)
    {
        var result = await _authService.Login(login);

        return Ok(result);
    }

    /// <summary>
    /// Registers a new user in the system.
    /// </summary>
    /// <param name="registration">The registration request containing username, email, and password.</param>
    /// <returns>
    /// Returns <see cref="Ok"/> if registration is successful. 
    /// Returns <see cref="ValidationProblem"/> if validation errors occur.
    /// </returns>
    /// <response code="200">Registration successful.</response>
    /// <response code="400">Validation failed (e.g., duplicate username, invalid email, weak password).</response>
    [HttpPost("register")]
    public async Task<Results<Ok, ValidationProblem>> Register(RegisterRequest registration)
    {
        var result = await _authService.Register(registration);

        if (!result.Succeeded)
        {
            return CreateValidationProblem(result);
        }

        return TypedResults.Ok();
    }

    private static ValidationProblem CreateValidationProblem(IdentityResult result)
    {
        // We expect a single error code and description in the normal case.
        // This could be golfed with GroupBy and ToDictionary, but perf! :P
        Debug.Assert(!result.Succeeded);
        var errorDictionary = new Dictionary<string, string[]>(1);

        foreach (var error in result.Errors)
        {
            string[] newDescriptions;

            if (errorDictionary.TryGetValue(error.Code, out var descriptions))
            {
                newDescriptions = new string[descriptions.Length + 1];
                Array.Copy(descriptions, newDescriptions, descriptions.Length);
                newDescriptions[descriptions.Length] = error.Description;
            }
            else
            {
                newDescriptions = [error.Description];
            }

            errorDictionary[error.Code] = newDescriptions;
        }

        return TypedResults.ValidationProblem(errorDictionary);
    }
}
