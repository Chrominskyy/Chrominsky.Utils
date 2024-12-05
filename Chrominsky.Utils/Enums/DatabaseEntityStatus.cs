using System.Text.Json.Serialization;

namespace Chrominsky.Utils.Enums;

/// <summary>
/// Represents the status of a db entity.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DatabaseEntityStatus
{
    /// <summary>
    /// The db entity is active and can be used.
    /// </summary>
    Active,

    /// <summary>
    /// The db entity is inactive and cannot be used.
    /// </summary>
    Inactive,

    /// <summary>
    /// The db entity has been deleted and cannot be used.
    /// </summary>
    Deleted,

    /// <summary>
    /// The db entity is in draft mode and has not been published yet.
    /// </summary>
    Draft,
}