using System.Text;

namespace Chrominsky.Utils.Models;

/// <summary>
/// Represents a paginated request with a generic type parameter.
/// </summary>
/// <typeparam name="T">The type of data to be paginated.</typeparam>
public class PaginatedRequest<T>
{
    /// <summary>
    /// Gets or sets the page number for the paginated request.
    /// </summary>
    public int Page { get; set; }
    
    /// <summary>
    /// Gets or sets the number of items per page for the paginated request.
    /// </summary>
    public int PageSize { get; set; }
    
    /// <summary>
    /// Gets or sets the data to be paginated.
    /// </summary>
    public T Data { get; set; }
}