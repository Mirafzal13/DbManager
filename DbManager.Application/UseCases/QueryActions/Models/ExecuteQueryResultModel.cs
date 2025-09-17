namespace DbManager.Application.UseCases.QueryActions.Models
{
    public class ExecuteQueryResultModel
    {
        public List<string> Columns { get; set; } = new();
        public List<List<object?>> Rows { get; set; } = new();
        public int AffectedRows { get; set; }
        public bool IsSelect { get; set; }
    }
}
