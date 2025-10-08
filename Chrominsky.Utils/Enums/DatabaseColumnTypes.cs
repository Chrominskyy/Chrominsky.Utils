namespace Chrominsky.Utils.Enums;

public class DatabaseColumnTypes
{
    public static List<string> Text { get; } =
        ["char", "nchar", "varchar", "nvarchar", "text", "ntext", "character varying", "character", "bpchar"];
    public static List<string> Number { get; } =
        [
            "int",
            "smallint",
            "bigint",
            "tinyint",
            "decimal",
            "numeric",
            "float",
            "real",
            "money",
            "smallmoney",
            "integer",
            "int2",
            "int4",
            "int8",
            "float4",
            "float8",
            "double precision",
        ];
    public static List<string> Date { get; } =
        ["date", "datetime", "datetime2", "smalldatetime", "datetimeoffset", "time", "timestamp", "timestamp without time zone", "timestamp with time zone", "timestamptz", "time without time zone", "time with time zone", "timetz", "interval"];
    public static List<string> Binary { get; } = ["binary", "varbinary", "image", "bytea"];
    public static List<string> Boolean { get; } = ["bit", "boolean", "bool"];
    public static List<string> Lookup { get; } = ["uniqueidentifier", "uuid"];

    /// <summary>
    /// Returns the group name that the specified data type belongs to.
    /// Returns null if the data type is not found.
    /// </summary>
    public static string? GetGroup(string dataType)
    {
        // Normalize the input for comparison.
        var type = dataType.ToLowerInvariant();

        if (Text.Contains(type))
            return "Text";
        if (Number.Contains(type))
            return "Number";
        if (Date.Contains(type))
            return "Date";
        if (Binary.Contains(type))
            return "Binary";
        if (Boolean.Contains(type))
            return "Boolean";
        if (Lookup.Contains(type))
            return "Lookup";

        return null;
    }
}
