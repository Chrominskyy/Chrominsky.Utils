using System.Text.Json;
using Chrominsky.Utils.Dto;
using Chrominsky.Utils.Models;
using TableColumn = Chrominsky.Utils.Dto.TableColumn;

namespace Chrominsky.Utils.Mappers;

public class TableColumnsMapper
{
    public TableColumnsDto ToDto(TableColumns tableColumns)
    {
        return new TableColumnsDto
        {
            TableName = tableColumns.TableName,
            Columns = tableColumns.Columns.Select(c => new TableColumn
            {
                Name = c.ColumnName,
                Type = c.Type,
                Order = c.Order,
                DefaultValue = c.DefaultValue,
                MaxLength = c.MaxLength,
                Nullable = c.Nullable,
            }).ToList(),
        };
    }

    public TableColumns ToModel(TableColumnsDto tableColumnsDto)
    {
        return new TableColumns
        {
            TableName = tableColumnsDto.TableName,
            Json = JsonSerializer.Serialize(tableColumnsDto.Columns.Select(c => new TableColumn
            {
                Type = c.Type,
                Order = c.Order,
                DefaultValue = c.DefaultValue,
                MaxLength = c.MaxLength,
                Nullable = c.Nullable,
                Name = c.Name,
            }).ToList()),
        };
    }
}