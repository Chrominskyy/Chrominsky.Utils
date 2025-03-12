namespace Chrominsky.Utils.Dto;

public class TableColumnsDto
{
    public string TableName { get; set; }
    public List<TableColumn> Columns { get; set; }
}

public class TableColumn
{
    public string Name { get; set; }
    public string Type { get; set; }
    public int Order { get; set; }
    public string DefaultValue { get; set; }
    public int MaxLength { get; set; }
    public bool Nullable { get; set; }
}