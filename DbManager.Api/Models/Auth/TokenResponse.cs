namespace DbManager.Api.Models.Auth;

using System.Text.Json.Serialization;

public class TokenResponse
{
    [JsonPropertyName("accessToken")]
    public required string AccessToken { get; set; }

    [JsonPropertyName("refreshToken")]
    public required string RefreshToken { get; set; }
}
