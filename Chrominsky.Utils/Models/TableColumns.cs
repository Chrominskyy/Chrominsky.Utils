using System.Text.Json;

namespace Chrominsky.Utils.Models;

public class TableColumns
{
    public string TableName { get; set; }

    public string Json { get; set; }

    public List<TableColumn> Columns => JsonSerializer.Deserialize<List<TableColumn>>(Json) ?? [];
}

public class TableColumn
{
    public string ColumnName { get; set; }
    public string Type { get; set; }
    public int Order { get; set; }
    public string DefaultValue { get; set; }
    public int MaxLength { get; set; }
    public int IsNullable { get; set; }

    public bool Nullable
    {
        get => IsNullable == 0;
    }
}