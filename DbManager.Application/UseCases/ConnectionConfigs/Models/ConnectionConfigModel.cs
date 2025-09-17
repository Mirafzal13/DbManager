namespace DbManager.Application.UseCases.ConnectionConfigs.Models;

public class ConnectionConfigModel
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Host { get; set; }
    public int Port { get; set; }
    public string? Database { get; set; }
    public required string Username { get; set; }
    public required string EncryptedPassword { get; set; }
}
