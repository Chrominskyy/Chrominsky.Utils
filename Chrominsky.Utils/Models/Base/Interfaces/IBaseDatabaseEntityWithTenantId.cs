namespace Chrominsky.Utils.Models.Base.Interfaces;

public interface IBaseDatabaseEntityWithTenantId : IBaseDatabaseEntity
{
    /// <summary>
    /// Identifier of the tenant to which the entity belongs. Nullable.
    /// </summary>
    Guid? TenantId { get; set; }
}