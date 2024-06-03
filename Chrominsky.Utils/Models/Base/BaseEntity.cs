using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Chrominsky.Utils.Enums;
using Newtonsoft.Json;

namespace Chrominsky.Utils.Models.Base;

/// <summary>
/// Represents a base class for database entities.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier of the entity.
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// The unique identifier of the tenant.
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public Guid? TenantId { get; set; }

    /// <summary>
    /// The date and time when the entity was created.
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// The date and time when the entity was last updated.
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// The user who created the entity.
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public string? CreatedBy { get; set; }

    /// <summary>
    /// The user who last updated the entity.
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Indicates whether the entity has been deleted.
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    [Required]
    [Newtonsoft.Json.JsonConverter(typeof(JsonStringEnumConverter))]
    public DatabaseEntityStatus? Status { get; set; }
}