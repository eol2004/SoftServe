using System.Collections.Generic;

namespace VirtoCommerce.Platform.Core.DynamicProperties
{
    public interface IHasDynamicProperties : IHasDynamicPropertiesBase
    {
        string Id { get; set; }
    }
}
