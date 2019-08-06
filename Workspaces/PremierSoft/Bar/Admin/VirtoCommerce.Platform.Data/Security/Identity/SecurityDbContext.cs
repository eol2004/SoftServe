using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace VirtoCommerce.Platform.Data.Security.Identity
{
    public class SecurityDbContext : IdentityDbContext<ApplicationUser>
    {
        public SecurityDbContext()
            : this("BarEntities")
        {
        }

        public SecurityDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString, false)
        {
            Database.SetInitializer<SecurityDbContext>(null);
        }
    }
}
