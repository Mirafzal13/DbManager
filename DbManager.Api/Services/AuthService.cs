namespace DbManager.Api.Services;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using DbManager.Api.Models.Auth;
using DbManager.Api.Utils;
using DbManager.Application.Common.Abstractions;

public interface IAuthService
{
    Task<IdentityResult> Register(RegisterRequest registration);
    Task<TokenResponse> Login(LoginRequest login);
}

public class AuthService(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    IUserStore<User> userStore,
    IAppDbContext dbContext) : IAuthService
{
    // Validate the email address using DataAnnotations like the UserValidator does when RequireUniqueEmail = true.
    private static readonly EmailAddressAttribute EmailAddressAttribute = new();



    public async Task<IdentityResult> Register(RegisterRequest registration)
    {
        if (!userManager.SupportsUserEmail)
        {
            throw new NotSupportedException($"{nameof(AuthService)} requires a user store with email support.");
        }

        var emailStore = (IUserEmailStore<User>)userStore;
        var email = registration.Email;
        var userName = registration.UserName;

        if (string.IsNullOrEmpty(email) || !EmailAddressAttribute.IsValid(email))
        {
            return IdentityResult.Failed(userManager.ErrorDescriber.InvalidEmail(email));
        }

        var user = new User();
        await userStore.SetUserNameAsync(user, userName, CancellationToken.None);
        await emailStore.SetEmailAsync(user, email, CancellationToken.None);
        var result = await userManager.CreateAsync(user, registration.Password);

        await dbContext.SaveChangesAsync(CancellationToken.None);

        return result;
    }

    public async Task<TokenResponse> Login(LoginRequest login)
    {
        var user = await userManager.FindByNameAsync(login.UserName)
            ?? throw new AccessDeniedException("Username incorrect.");

        var result = await signInManager.PasswordSignInAsync(login.UserName, login.Password, isPersistent: false, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            throw new AccessDeniedException("Password incorrect.");
        }

        // Generate tokens

        var accessToken = await TokenUtils.GenerateAccessToken(user, "C7D4B9E1D5A048E59EF175DE1E88B81C3FD0B8076A9A382F6CD1928BEFE88B89", userManager);
        var refreshToken = TokenUtils.GenerateRefreshToken();

        return new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }
}
