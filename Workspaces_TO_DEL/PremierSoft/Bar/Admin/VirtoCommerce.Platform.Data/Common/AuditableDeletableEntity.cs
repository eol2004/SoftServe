using System;
using System.ComponentModel;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Data.Common
{
    public abstract class AuditableDeletableEntity : AuditableEntityInt, IDeletableEntity
    {
        [DefaultValue(false)]
        public bool Deleted { get; set; }
    }
}