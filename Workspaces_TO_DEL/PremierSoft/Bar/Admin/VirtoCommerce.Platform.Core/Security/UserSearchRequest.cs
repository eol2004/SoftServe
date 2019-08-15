using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Core.Security
{
    public class UserSearchRequest
    {
        public UserSearchRequest()
        {
            TakeCount = 20;
        }

        public string[] AccountTypes { get; set; }
        public string Keyword { get; set; }
        public string MemberId { get; set; }
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
