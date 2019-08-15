using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Common;

namespace VirtoCommerce.Platform.Data.Model
{
    public class AccountEntity : AuditableEntity, IDeletableEntity
    {
        public AccountEntity()
        {
            RoleAssignments = new NullCollection<RoleAssignmentEntity>();
            ApiAccounts = new NullCollection<ApiAccountEntity>();
        }
        [Index]
        public int? CompanyId { get; set; }       // PS Вместо StoreId
        /// <summary>
        /// Идентификатор для доступа к службе внешних продаж кино через API
        /// </summary>
        public System.Guid? CinemaServiceId { get; set; }
        [StringLength(64)]
        public string MemberId { get; set; }
        //[Required]
        [StringLength(128)]
        [Index]
        public string UserName { get; set; }
        public bool IsAdministrator { get; set; }
        [StringLength(128)]
        public string UserType { get; set; }
        [StringLength(128)]
        public string AccountState { get; set; }
        /// <summary>
        /// Имя пользователя в системе
        /// </summary>
        [Required]
        [StringLength(256)]
        [Index]
        public string DisplayName { get; set; }

        [DefaultValue(false)]
        public bool Deleted { get; set; }

        public virtual ObservableCollection<RoleAssignmentEntity> RoleAssignments { get; set; }
        public virtual ObservableCollection<ApiAccountEntity> ApiAccounts { get; set; }
    }
}
