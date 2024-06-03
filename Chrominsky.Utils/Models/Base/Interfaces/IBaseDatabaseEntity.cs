using Chrominsky.Utils.Enums;

namespace Chrominsky.Utils.Models.Base.Interfaces;

/// <summary>
/// Interface for defining common properties for database entities.
/// </summary>
public interface IBaseDatabaseEntity
{
    /// <summary>
    /// Unique identifier for the entity.
    /// </summary>
    Guid Id { get; set; }

    /// <summary>
    /// Date and time when the entity was created.
    /// </summary>
    DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date and time when the entity was last updated. Nullable.
    /// </summary>
    DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Identifier of the user who created the entity.
    /// </summary>
    string CreatedBy { get; set; }

    /// <summary>
    /// Identifier of the user who last updated the entity. Nullable.
    /// </summary>
    string? UpdatedBy { get; set; }

    /// <summary>
    /// Status of the entity in the database.
    /// </summary>
    DatabaseEntityStatus Status { get; set; }
}