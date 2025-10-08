using Chrominsky.Utils.Enums;
using Xunit;

namespace Chrominsky.Utils.Tests.Enums;

public class DatabaseColumnTypesTests
{
    [Theory]
    [InlineData("varchar", "Text")]
    [InlineData("nvarchar", "Text")]
    [InlineData("text", "Text")]
    [InlineData("char", "Text")]
    [InlineData("nchar", "Text")]
    [InlineData("ntext", "Text")]
    public void GetGroup_SqlServerTextTypes_ReturnsText(string dataType, string expectedGroup)
    {
        // Act
        var result = DatabaseColumnTypes.GetGroup(dataType);

        // Assert
        Assert.Equal(expectedGroup, result);
    }

    [Theory]
    [InlineData("character varying", "Text")]
    [InlineData("character", "Text")]
    [InlineData("bpchar", "Text")]
    public void GetGroup_PostgreSqlTextTypes_ReturnsText(string dataType, string expectedGroup)
    {
        // Act
        var result = DatabaseColumnTypes.GetGroup(dataType);

        // Assert
        Assert.Equal(expectedGroup, result);
    }

    [Theory]
    [InlineData("int", "Number")]
    [InlineData("bigint", "Number")]
    [InlineData("smallint", "Number")]
    [InlineData("tinyint", "Number")]
    [InlineData("decimal", "Number")]
    [InlineData("numeric", "Number")]
    [InlineData("float", "Number")]
    [InlineData("real", "Number")]
    [InlineData("money", "Number")]
    [InlineData("smallmoney", "Number")]
    public void GetGroup_SqlServerNumberTypes_ReturnsNumber(string dataType, string expectedGroup)
    {
        // Act
        var result = DatabaseColumnTypes.GetGroup(dataType);

        // Assert
        Assert.Equal(expectedGroup, result);
    }

    [Theory]
    [InlineData("integer", "Number")]
    [InlineData("int2", "Number")]
    [InlineData("int4", "Number")]
    [InlineData("int8", "Number")]
    [InlineData("float4", "Number")]
    [InlineData("float8", "Number")]
    [InlineData("double precision", "Number")]
    public void GetGroup_PostgreSqlNumberTypes_ReturnsNumber(string dataType, string expectedGroup)
    {
        // Act
        var result = DatabaseColumnTypes.GetGroup(dataType);

        // Assert
        Assert.Equal(expectedGroup, result);
    }

    [Theory]
    [InlineData("date", "Date")]
    [InlineData("datetime", "Date")]
    [InlineData("datetime2", "Date")]
    [InlineData("smalldatetime", "Date")]
    [InlineData("datetimeoffset", "Date")]
    [InlineData("time", "Date")]
    public void GetGroup_SqlServerDateTypes_ReturnsDate(string dataType, string expectedGroup)
    {
        // Act
        var result = DatabaseColumnTypes.GetGroup(dataType);

        // Assert
        Assert.Equal(expectedGroup, result);
    }

    [Theory]
    [InlineData("timestamp", "Date")]
    [InlineData("timestamp without time zone", "Date")]
    [InlineData("timestamp with time zone", "Date")]
    [InlineData("timestamptz", "Date")]
    [InlineData("time without time zone", "Date")]
    [InlineData("time with time zone", "Date")]
    [InlineData("timetz", "Date")]
    [InlineData("interval", "Date")]
    public void GetGroup_PostgreSqlDateTypes_ReturnsDate(string dataType, string expectedGroup)
    {
        // Act
        var result = DatabaseColumnTypes.GetGroup(dataType);

        // Assert
        Assert.Equal(expectedGroup, result);
    }

    [Theory]
    [InlineData("binary", "Binary")]
    [InlineData("varbinary", "Binary")]
    [InlineData("image", "Binary")]
    public void GetGroup_SqlServerBinaryTypes_ReturnsBinary(string dataType, string expectedGroup)
    {
        // Act
        var result = DatabaseColumnTypes.GetGroup(dataType);

        // Assert
        Assert.Equal(expectedGroup, result);
    }

    [Theory]
    [InlineData("bytea", "Binary")]
    public void GetGroup_PostgreSqlBinaryTypes_ReturnsBinary(string dataType, string expectedGroup)
    {
        // Act
        var result = DatabaseColumnTypes.GetGroup(dataType);

        // Assert
        Assert.Equal(expectedGroup, result);
    }

    [Theory]
    [InlineData("bit", "Boolean")]
    public void GetGroup_SqlServerBooleanTypes_ReturnsBoolean(string dataType, string expectedGroup)
    {
        // Act
        var result = DatabaseColumnTypes.GetGroup(dataType);

        // Assert
        Assert.Equal(expectedGroup, result);
    }

    [Theory]
    [InlineData("boolean", "Boolean")]
    [InlineData("bool", "Boolean")]
    public void GetGroup_PostgreSqlBooleanTypes_ReturnsBoolean(string dataType, string expectedGroup)
    {
        // Act
        var result = DatabaseColumnTypes.GetGroup(dataType);

        // Assert
        Assert.Equal(expectedGroup, result);
    }

    [Theory]
    [InlineData("uniqueidentifier", "Lookup")]
    public void GetGroup_SqlServerLookupTypes_ReturnsLookup(string dataType, string expectedGroup)
    {
        // Act
        var result = DatabaseColumnTypes.GetGroup(dataType);

        // Assert
        Assert.Equal(expectedGroup, result);
    }

    [Theory]
    [InlineData("uuid", "Lookup")]
    public void GetGroup_PostgreSqlLookupTypes_ReturnsLookup(string dataType, string expectedGroup)
    {
        // Act
        var result = DatabaseColumnTypes.GetGroup(dataType);

        // Assert
        Assert.Equal(expectedGroup, result);
    }

    [Theory]
    [InlineData("VARCHAR", "Text")]
    [InlineData("INTEGER", "Number")]
    [InlineData("BOOLEAN", "Boolean")]
    [InlineData("UUID", "Lookup")]
    public void GetGroup_UpperCaseTypes_ReturnsCorrectGroup(string dataType, string expectedGroup)
    {
        // Act
        var result = DatabaseColumnTypes.GetGroup(dataType);

        // Assert
        Assert.Equal(expectedGroup, result);
    }

    [Theory]
    [InlineData("unknown_type")]
    [InlineData("custom_type")]
    [InlineData("")]
    public void GetGroup_UnknownTypes_ReturnsNull(string dataType)
    {
        // Act
        var result = DatabaseColumnTypes.GetGroup(dataType);

        // Assert
        Assert.Null(result);
    }
}
