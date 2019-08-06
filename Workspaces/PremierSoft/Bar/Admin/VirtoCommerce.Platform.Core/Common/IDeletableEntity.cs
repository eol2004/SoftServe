using System;

namespace VirtoCommerce.Platform.Core.Common
{
    public interface IDeletableEntity
    {
        bool Deleted { get; set; }
    }
}
