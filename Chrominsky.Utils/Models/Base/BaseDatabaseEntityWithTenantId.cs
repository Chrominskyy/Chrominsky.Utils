using System.ComponentModel.DataAnnotations;
using Chrominsky.Utils.Models.Base.Interfaces;

namespace Chrominsky.Utils.Models.Base;

public class BaseDatabaseEntityWithTenantId : BaseDatabaseEntity, IBaseDatabaseEntityWithTenantId
{
    /// <inheritdoc />
    [Required]
    public Guid? TenantId { get; set; }
}