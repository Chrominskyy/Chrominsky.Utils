using System.ComponentModel.DataAnnotations;

namespace Chrominsky.Utils.Models.Base;

public class ObjectVersion
{
    /// <summary>
    /// Unique identifier for the object versioning record.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Type of the object being versioned.
    /// </summary>
    [Required]
    public string ObjectType { get; set; }

    /// <summary>
    /// Identifier of the object being versioned.
    /// </summary>
    [Required]
    public Guid ObjectId { get; set; }

    /// <summary>
    /// Identifier of the tenant associated with the object.
    /// </summary>
    [Required]
    public Guid ObjectTenant { get; set; }

    /// <summary>
    /// Value of the object before the update.
    /// </summary>
    public string? BeforeValue { get; set; }

    /// <summary>
    /// Value of the object after the update.
    /// </summary>
    [Required]
    public string? AfterValue { get; set; }

    /// <summary>
    /// Timestamp of when the object was updated.
    /// </summary>
    [Required]
    public DateTime UpdatedOn { get; set; }

    /// <summary>
    /// Identifier of the user who performed the update.
    /// </summary>
    [Required]
    public string UpdatedBy { get; set; }

    public ObjectVersion()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectVersion"/> class.
    /// </summary>
    /// <param name="objectType">Type of the object being versioned.</param>
    /// <param name="objectId">Identifier of the object being versioned.</param>
    /// <param name="objectTenant">Identifier of the tenant associated with the object.</param>
    /// <param name="beforeValue">Value of the object before the update. Can be null.</param>
    /// <param name="afterValue">Value of the object after the update.</param>
    /// <param name="updatedBy">Identifier of the user who performed the update.</param>
    public ObjectVersion(
        string objectType,
        Guid objectId,
        Guid objectTenant,
        string? beforeValue,
        string? afterValue,
        string updatedBy
    )
    {
        Id = Guid.NewGuid();
        ObjectType = objectType;
        ObjectId = objectId;
        ObjectTenant = objectTenant;
        BeforeValue = beforeValue;
        AfterValue = afterValue;
        UpdatedOn = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
}