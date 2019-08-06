using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Data.Common
{
    /// <summary>
    /// Элемент базы данных, доступ к которому предоставлен только из определенной организации
    /// </summary>
    public abstract class AuditableCompanyEntity : AuditableDeletableEntity, ICompanyEntity
    {
        [Index]
        public int CompanyId { get; set; }
    }
}