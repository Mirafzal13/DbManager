namespace DbManager.Application.UseCases.ConnectionConfigs.Models
{
    public class ActiveConnectionModel
    {
        public Guid ConnectionId { get; set; }
        public string? ServerName { get; set; }
    }
}
