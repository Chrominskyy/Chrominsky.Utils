namespace Chrominsky.Utils.Models;

/// <summary>
/// Represents a paginated response containing data of type T.
/// </summary>
/// <typeparam name="T">The type of data in the response.</typeparam>
public class PaginatedResponse<T>
{
    /// <summary>
    /// Gets or sets the current page number.
    /// </summary>
    public int Page { get; set; }
    
    /// <summary>
    /// Gets or sets the number of items per page.
    /// </summary>
    public int PageSize { get; set; }
    
    /// <summary>
    /// Gets or sets the total number of items across all pages.
    /// </summary>
    public int TotalCount { get; set; }
    
    /// <summary>
    /// Gets or sets the data for the current page.
    /// </summary>
    public T Data { get; set; }
}