using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace VirtoCommerce.Platform.Core.Security
{
    public class ApplicationUserExtended
    {
        public string Id { get; set; }
        /// <summary>
        /// Имя пользователя для входа на сайт
        /// </summary>
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Tenant id
        /// </summary>
        public int? CompanyId { get; set; }       // PS Вместо StoreId
        /// <summary>
        /// Идентификатор для доступа к службе внешних продаж кино через API
        /// </summary>
        public System.Guid? CinemaServiceId { get; set; }
        public string MemberId { get; set; }
        public string Icon { get; set; }

        public bool IsAdministrator { get; set; }

        public string UserType { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public AccountState UserState { get; set; }

        public string Password { get; set; }

        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        /// <summary>
        /// Имя пользователя в системе
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// ПИН-код для входа на кассу
        /// </summary>
        public string PinCode { get; set; }
        /// <summary>
        /// Номер карты для входа на кассу
        /// </summary>
        public long? CardCode { get; set; }
        /// <summary>
        /// External provider logins.
        /// </summary>
        public ApplicationUserLogin[] Logins { get; set; }

        /// <summary>
        /// Assigned roles.
        /// </summary>
        public Role[] Roles { get; set; }

        /// <summary>
        /// All permissions from assigned roles.
        /// </summary>
        public string[] Permissions { get; set; }

        /// <summary>
        /// API keys
        /// </summary>
        public ApiAccount[] ApiAccounts { get; set; }
    }
}
