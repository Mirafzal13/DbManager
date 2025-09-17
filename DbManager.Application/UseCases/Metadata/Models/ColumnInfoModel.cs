namespace DbManager.Application.UseCases.Metadata.Models
{
    public class ColumnInfoModel
    {
        public string Name { get; set; } = default!;
        public string DataType { get; set; } = default!;
        public bool IsNullable { get; set; }
        public int? MaxLength { get; set; }
        public bool IsPrimaryKey { get; set; }
    }
}
