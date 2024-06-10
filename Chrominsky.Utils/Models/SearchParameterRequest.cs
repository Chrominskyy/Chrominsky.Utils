namespace Chrominsky.Utils.Models;

/// <summary>
/// Represents a request for searching with pagination and specific parameters.
/// </summary>
public class SearchParameterRequest
{
    /// <summary>
    /// Gets or sets the page number for pagination.
    /// </summary>
    public int Page { get; set; }
    
    /// <summary>
    /// Gets or sets the number of items per page for pagination.
    /// </summary>
    public int PageSize { get; set; }
    
    /// <summary>
    /// Gets or sets the collection of search parameters to apply to the search.
    /// </summary>
    public IEnumerable<SearchParameter> SearchParameters { get; set; }
}