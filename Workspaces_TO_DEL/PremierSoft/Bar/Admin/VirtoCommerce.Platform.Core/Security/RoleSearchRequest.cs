using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Core.Security
{
    public class RoleSearchRequest
    {
        public RoleSearchRequest()
        {
            TakeCount = 20;
        }

        public string Keyword { get; set; }
        public int SkipCount { get; set; }
        public int TakeCount { get; set; }

        /// <summary>
        /// Sorting expression property1:asc;property2:desc
        /// </summary>
        public string Sort { get; set; }

        public SortInfo[] SortInfos
        {
            get
            {
                return SortInfo.Parse(Sort).ToArray();
            }
        }
    }
}
