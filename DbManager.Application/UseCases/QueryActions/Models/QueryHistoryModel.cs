namespace DbManager.Application.UseCases.QueryActions.Models
{
    public class QueryHistoryModel
    {
        public Guid Id { get; set; }
        public Guid ConnectionId { get; set; }
        public string Database { get; set; } = default!;
        public string Sql { get; set; } = default!;
        public DateTime ExecutedAt { get; set; }
    }
}
