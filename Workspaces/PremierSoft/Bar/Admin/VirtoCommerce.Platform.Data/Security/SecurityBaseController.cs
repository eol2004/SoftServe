using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Data.Common;

namespace VirtoCommerce.Platform.Data.Security
{
    public abstract class SecurityBaseController : ApiController
    {
        private readonly ISecurityService _securityService;

        protected ISecurityService SecurityService
        {
            get { return _securityService; }
        }

        private readonly IPermissionScopeService _permissionScopeService;

        protected IPermissionScopeService PermissionScopeService
        {
            get { return _permissionScopeService; }
        }

        public SecurityBaseController(ISecurityService securityService, IPermissionScopeService permissionScopeService)
        {
            _securityService = securityService;
            _permissionScopeService = permissionScopeService;
        }

        protected string[] GetObjectPermissionScopeStrings(object obj)
        {
            return _permissionScopeService.GetObjectPermissionScopeStrings(obj).ToArray();
        }

        protected void CheckCurrentUserHasPermissionForObjects(string permission, params object[] objects)
        {
            if (objects == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            var name = User.Identity.Name;
            //Scope bound security check
            var scopes = objects.SelectMany(x => _permissionScopeService.GetObjectPermissionScopeStrings(x)).Distinct().ToArray();
            if (!_securityService.UserHasAnyPermission(name, scopes, permission))
            {
                var companyItems = objects.OfType<ICompanyEntity>();
                if (companyItems.Any())
                {
                    int companyId;
                    if (_securityService.IsUserCompanyManager(name, out companyId))
                    {
                        if (companyItems.All(x => x.CompanyId == companyId))
                            return;
                    }
                }

                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
        }

        protected void UpdateCompanyId(ICompanyEntity entity)
        {
            var name = User.Identity.Name;

            int companyId;
            if (_securityService.IsUserCompanyManager(name, out companyId))
            {
                entity.CompanyId = companyId;
            }
        }

        protected void UpdateCompanyId(ICompanyNullableEntity entity)
        {
            var name = User.Identity.Name;

            int companyId;
            if (_securityService.IsUserCompanyManager(name, out companyId))
            {
                entity.CompanyId = companyId;
            }
        }

        //protected void UpdateCompanyId(object entity)
        //{
        //    var companyEntity = entity as IAuditableCompanyEntity;
        //    if (companyEntity == null)
        //        return;

        //    var name = User.Identity.Name;

        //    int companyId;
        //    if (_securityService.IsUserCompanyManager(name, out companyId))
        //    {
        //        if (string.IsNullOrEmpty(companyId))    // Возможно для администратора сайте без определенного предприятия. Сохраняем информацию о предприятии неизменной.
        //            return;
        //        companyEntity.CompanyId = companyId;
        //    }
        //}

        protected int GetCompanyId()
        {
            var name = User.Identity.Name;
            return _securityService.GetCompanyId(name) ?? default(int);
        }

        /// <summary>
        /// Filter items based on current user permissions
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        protected IEnumerable<TEntity> ApplyRestrictionsForCurrentUser<TEntity>(IEnumerable<TEntity> items, string readPermission)
            where TEntity : ICompanyEntity
        {
            var name = User.Identity.Name;

            // Check global permission
            if (!_securityService.UserHasAnyPermission(name, null, readPermission))
                return Enumerable.Empty<TEntity>();

            int companyId;
            if (!_securityService.IsUserCompanyManager(name, out companyId))
                return Enumerable.Empty<TEntity>();

            return items.Where(x => x.CompanyId == companyId).ToList();
        }

        /// <summary>
        /// Filter items based on current user permissions
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        protected IEnumerable<TEntity> ApplyRestrictionsForCurrentUser<TEntity>(IEnumerable<TEntity> items, string readPermission, bool allowNullCompany = false)
            where TEntity : ICompanyNullableEntity
        {
            var name = User.Identity.Name;

            // Check global permission
            if (!_securityService.UserHasAnyPermission(name, null, readPermission))
                return Enumerable.Empty<TEntity>();

            int companyId;
            if (!_securityService.IsUserCompanyManager(name, out companyId))
                return Enumerable.Empty<TEntity>();

            if (allowNullCompany)
                return items.Where(x => !x.CompanyId.HasValue || x.CompanyId == companyId).ToList();

            return items.Where(x => x.CompanyId == companyId).ToList();
        }

        /// <summary>
        /// Filter items based on current user permissions
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        protected IEnumerable<TEntity> ApplyRestrictionsForCurrentUser<TEntity, TPermissionScope>(IEnumerable<TEntity> items, string readPermission)
            where TEntity : ICompanyEntity, Core.Common.IEntityInt
            where TPermissionScope : PermissionScope
        {
            var name = User.Identity.Name;

            // Check global permission
            if (_securityService.UserHasAnyPermission(name, null, readPermission))
                return items;

            int companyId;
            List<string> companyItemsId;
            if (_securityService.IsUserCompanyManager(name, out companyId))
                companyItemsId = items.Where(x => x.CompanyId == companyId)
                    .Select(x => x.Id.ToString())
                    .ToList();
            else
                companyItemsId = new List<string>();

            // Get user 'read' permission scopes
            var readPermissionScopes = _securityService.GetUserPermissions(name)
                .Where(x => x.Id.StartsWith(readPermission))
                .SelectMany(x => x.AssignedScopes)
                .ToList();

            var filterIds = readPermissionScopes.OfType<TPermissionScope>()
                .Select(x => x.Scope)
                .Where(x => !string.IsNullOrEmpty(x))
                .Union(companyItemsId)
                .ToArray();

            var result = items.Where(x => filterIds.Contains(x.Id.ToString()));
            return result.ToList();
        }

        /// <summary>
        /// Filter items based on current user permissions
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        protected IEnumerable<TEntity> ApplyRestrictionsForCurrentUser<TEntity, TPermissionScope1, TPermissionScope2>(IEnumerable<TEntity> items, string readPermission)
            where TEntity : ICompanyNullableEntity, Core.Common.IEntityInt
            where TPermissionScope1 : PermissionScope
            where TPermissionScope2 : PermissionCompanyScope
        {
            var name = User.Identity.Name;

            // Check global permission
            if (_securityService.UserHasAnyPermission(name, null, readPermission))
                return items;

            // Get user 'read' permission scopes
            var readPermissionScopes = _securityService.GetUserPermissions(name)
                .Where(x => x.Id.StartsWith(readPermission))
                .SelectMany(x => x.AssignedScopes)
                .ToList();

            var filterIds1 = readPermissionScopes.OfType<TPermissionScope1>()
                .Select(x => x.Scope)
                .Where(x => !string.IsNullOrEmpty(x));
            var result1 = items.Where(x => filterIds1.Contains(x.Id.ToString()));

            var filterIds2 = readPermissionScopes.OfType<TPermissionScope2>()
                .Select(x => x.Scope)
                .Where(x => !string.IsNullOrEmpty(x));
            var result2 = items.Where(x => filterIds2.Contains(x.CompanyId.ToString()));

            var result = result1.Union(result2).ToList();
            return result;
        }

        /// <summary>
        /// Filter items based on current user permissions
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        protected IEnumerable<TEntity> ApplyRestrictionsByCompany<TEntity>(IEnumerable<TEntity> items, string readPermission)
            where TEntity : ICompanyEntity
        {
            var name = User.Identity.Name;

            // Check global permission
            if (_securityService.UserHasAnyPermission(name, null, readPermission))
                return items;

            int companyId;
            if (!_securityService.IsUserCompanyManager(name, out companyId))
                return Enumerable.Empty<TEntity>();

            return items.Where(x => x.CompanyId == companyId).ToList();
        }
    }
}
