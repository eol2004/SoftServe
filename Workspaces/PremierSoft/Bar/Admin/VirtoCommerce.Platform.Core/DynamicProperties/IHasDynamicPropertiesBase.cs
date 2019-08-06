using System.Collections.Generic;

namespace VirtoCommerce.Platform.Core.DynamicProperties
{
    public interface IHasDynamicPropertiesBase
    {
        string ObjectType { get; set; }
        ICollection<DynamicObjectProperty> DynamicProperties { get; set; }
    }
}
