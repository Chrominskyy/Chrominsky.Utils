using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Chrominsky.Utils.Enums;
using Chrominsky.Utils.Models.Base.Interfaces;
using Newtonsoft.Json;

namespace Chrominsky.Utils.Models.Base;

/// <summary>
/// Represents a base class for database entities.
/// </summary>
public abstract class BaseDatabaseEntity : IBaseDatabaseEntity
{
    /// <inheritdoc />
    [Required]
    [Key]
    public Guid Id { get; set; }

    /// <inheritdoc />
    [Required]
    public DateTime CreatedAt { get; set; }

    /// <inheritdoc />
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public DateTime? UpdatedAt { get; set; }

    /// <inheritdoc />
    [Required]
    public string CreatedBy { get; set; }

    /// <inheritdoc />
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public string? UpdatedBy { get; set; }

    /// <inheritdoc />
    [Required]
    [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
    public DatabaseEntityStatus Status { get; set; }
}