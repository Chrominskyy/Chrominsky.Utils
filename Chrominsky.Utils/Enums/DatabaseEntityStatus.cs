namespace Chrominsky.Utils.Enums;

/// <summary>
/// Represents the status of a promo code.
/// </summary>
public enum DatabaseEntityStatus
{
    /// <summary>
    /// The promo code is active and can be used.
    /// </summary>
    Active,

    /// <summary>
    /// The promo code is inactive and cannot be used.
    /// </summary>
    Inactive,

    /// <summary>
    /// The promo code has been deleted and cannot be used.
    /// </summary>
    Deleted,

    /// <summary>
    /// The promo code is in draft mode and has not been published yet.
    /// </summary>
    Draft,
}