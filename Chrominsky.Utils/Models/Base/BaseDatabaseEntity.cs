using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Chrominsky.Utils.Models.Base;

/// <summary>
/// Represents a base class for database entities.
/// </summary>
public abstract class BaseDatabaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier of the entity.
    /// </summary>
    [Required]
    public Guid Id { get; set; }
    
    /// <summary>
    /// The unique identifier of the tenant.
    /// </summary>
    [Required]
    public Guid? TenantId { get; set; }
    
    /// <summary>
    /// The date and time when the entity was created.
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// The date and time when the entity was last updated.
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// The user who created the entity.
    /// </summary>
    [Required]
    public string CreatedBy { get; set; }
    
    /// <summary>
    /// The user who last updated the entity.
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public string? UpdatedBy { get; set; }
    
    /// <summary>
    /// Indicates whether the entity has been deleted.
    /// </summary>
    public bool IsDeleted { get; set; }
}