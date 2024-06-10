using Chrominsky.Utils.Enums;

namespace Chrominsky.Utils.Models;

/// <summary>
/// Represents a search parameter used in a search request.
/// </summary>
public class SearchParameter
{
    /// <summary>
    /// Gets or sets the key of the search parameter.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Gets or sets the value of the search parameter.
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Gets or sets the search operator for the parameter <see cref="SearchOperator"/>.
    /// </summary>
    public SearchOperator Operator { get; set; }

    /// <summary>
    /// Gets or sets the order of the search results.
    /// If not provided, the default order is descending.
    /// <see cref="SearchOrder"/>
    /// </summary>
    public SearchOrder? Order { get; set; } = SearchOrder.Descending;
}