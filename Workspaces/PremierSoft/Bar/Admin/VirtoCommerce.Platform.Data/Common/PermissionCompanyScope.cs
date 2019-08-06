using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Data.Common;

namespace VirtoCommerce.Platform.Data.Common
{
    public class PermissionCompanyScope : PermissionScope
    {
        public override IEnumerable<string> GetEntityScopeStrings(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            int? companyId = null;
            var companyEntity = obj as ICompanyEntity;
            if (companyEntity != null)
                companyId = companyEntity.CompanyId;

            if (companyId.HasValue)
            {
                return new[] { Type + ":" + companyId.Value };
            }
            return Enumerable.Empty<string>();
        }
    }
}