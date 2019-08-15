using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace VirtoCommerce.Platform.Core.Common
{
    public abstract class EntityInt : IEntityInt
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public bool IsTransient()
        {
            return Id == default(int);
        }
    }
}
