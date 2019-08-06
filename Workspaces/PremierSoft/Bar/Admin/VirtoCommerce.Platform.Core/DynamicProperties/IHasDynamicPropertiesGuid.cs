using System;
using System.Collections.Generic;

namespace VirtoCommerce.Platform.Core.DynamicProperties
{
    public interface IHasDynamicPropertiesGuid : IHasDynamicPropertiesBase
    {
        Guid Guid { get; set; }
    }
}
