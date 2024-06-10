namespace Chrominsky.Utils.Enums;

/// <summary>
/// Represents the different operators that can be used in a search filter.
/// </summary>
public enum SearchOperator
{
    /// <summary>
    /// The filter will match any value that contains the specified search term.
    /// </summary>
    Contains,

    /// <summary>
    /// The filter will match only the values that exactly match the specified search term.
    /// </summary>
    Equals,

    /// <summary>
    /// The filter will match only the values that are less than the specified search term.
    /// </summary>
    LessThan,

    /// <summary>
    /// The filter will match only the values that are greater than the specified search term.
    /// </summary>
    GreaterThan,

    /// <summary>
    /// The filter will match only the values that are less than or equal to the specified search term.
    /// </summary>
    LessOrEqualThan,

    /// <summary>
    /// The filter will match only the values that are greater than or equal to the specified search term.
    /// </summary>
    GreaterOrEqualThan
}