namespace VirtoCommerce.Platform.Core.Security
{
    public enum AccountType
    {
        /// <summary>
        /// Сотрудник предприятия (не администратор)
        /// </summary>
        CompanyMember,
        /// <summary>
        /// Администратор предприятия. Для него доступны все элементы созданые в пределах связанного с ним предприятия
        /// </summary>
        CompanyManager,
        /// <summary>
        /// Администратор сайта
        /// </summary>
        Administrator
    }
}
