using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Chrominsky.Utils.Enums;
using Chrominsky.Utils.Models.Base.Interfaces;
using Newtonsoft.Json;

namespace Chrominsky.Utils.Models.Base;

public class BaseSimpleEntity : IBaseDatabaseEntity
{
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
    [Newtonsoft.Json.JsonConverter(typeof(JsonStringEnumConverter))]
    public DatabaseEntityStatus Status { get; set; }
}