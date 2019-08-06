using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Data.Common.Search
{
    /// <summary>
    /// Represent common search criteria. More specialized criteria should be derived for this type.
    /// </summary>
	public abstract class BasePaginationSearchCriteria
	{
        public BasePaginationSearchCriteria()
		{
            Take = 20;
		}

        /// <summary>
        /// Word, part of word or phrase to search
        /// </summary>
        public string Keyword { get; set; }

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

        public int Skip { get; set; }

        public int Take { get; set; }
    }
}
