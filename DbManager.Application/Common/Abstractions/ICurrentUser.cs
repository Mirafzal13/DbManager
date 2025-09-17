namespace DbManager.Application.Common;

public interface ICurrentUser
{
    public Guid UserId { get; }
    public string[] Roles { get; }
    public bool IsAdmin { get; }
}
