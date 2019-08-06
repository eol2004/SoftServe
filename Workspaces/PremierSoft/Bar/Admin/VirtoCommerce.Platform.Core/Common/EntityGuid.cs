using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace VirtoCommerce.Platform.Core.Common
{
    public abstract class EntityGuid
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
    }
}
