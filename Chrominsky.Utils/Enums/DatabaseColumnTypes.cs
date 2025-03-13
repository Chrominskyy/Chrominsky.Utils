namespace Chrominsky.Utils.Enums;

public class DatabaseColumnTypes
{
    public static List<string> Text { get; } =
        ["char", "nchar", "varchar", "nvarchar", "text", "ntext"];
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
        ];
    public static List<string> Date { get; } =
        ["date", "datetime", "datetime2", "smalldatetime", "datetimeoffset", "time"];
    public static List<string> Binary { get; } = ["binary", "varbinary", "image"];
    public static List<string> Boolean { get; } = ["bit"];
    public static List<string> Lookup { get; } = ["uniqueidentifier"];

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
