using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using CacheManager.Core;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Common;
using VirtoCommerce.Platform.Data.Model;
using VirtoCommerce.Platform.Data.Repositories;
using VirtoCommerce.Platform.Data.Settings.Converters;

namespace VirtoCommerce.Platform.Data.Settings
{
    public class SettingsManager : ISettingsManager
    {
        private readonly IModuleManifestProvider _manifestProvider;
        private readonly Func<IPlatformRepository> _repositoryFactory;
        private readonly ICacheManager<object> _cacheManager;
        private readonly ModuleManifest[] _predefinedManifests;
        private readonly IDictionary<string, List<SettingEntry>> _runtimeModuleSettingsMap = new Dictionary<string, List<SettingEntry>>();

        [CLSCompliant(false)]
        public SettingsManager(IModuleManifestProvider manifestProvider, Func<IPlatformRepository> repositoryFactory, ICacheManager<object> cacheManager, ModuleManifest[] predefinedManifests)
        {
            _manifestProvider = manifestProvider;
            _repositoryFactory = repositoryFactory;
            _cacheManager = cacheManager;
            _predefinedManifests = predefinedManifests ?? new ModuleManifest[0];
        }

        #region ISettingsManager Members

        public ModuleDescriptor[] GetModules()
        {
            var retVal = GetModuleManifestsWithSettings()
                .Select(x => x.ToModel())
                .ToArray();

            return retVal;
        }

        public SettingEntry GetSettingByName(string name, int? companyId = null)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            SettingEntry retVal = null;
            var manifestSetting = LoadSettingFromManifest(name);

            var settingEntities = GetAllEntities(companyId);
            var storedSetting = settingEntities.FirstOrDefault(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase) && s.CompanyId == companyId);

            if (storedSetting == null && companyId != null)
                storedSetting = settingEntities.FirstOrDefault(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase));

            if (manifestSetting != null)
            {
                retVal = manifestSetting.ToModel(storedSetting, null);
            }
            return retVal;
        }

        public void LoadEntitySettingsValues(Entity entity, int? companyId)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (entity.IsTransient())
                throw new ArgumentException("entity transistent");

            var storedSettings = new List<SettingEntry>();
            var entityType = entity.GetType().Name;
            using (var repository = _repositoryFactory())
            {
                var settings = repository.Settings
                    .Include(s => s.SettingValues)
                    .Where(x => x.ObjectId == entity.Id && x.ObjectType == entityType && x.CompanyId == companyId)
                    .OrderBy(x => x.Name)
                    .ToList();

                storedSettings.AddRange(settings.Select(x => x.ToModel()));
            }

            //Deep load settings values for all object contains settings
            var haveSettingsObjects = entity.GetFlatObjectsListWithInterface<IHaveSettings>();
            foreach (var haveSettingsObject in haveSettingsObjects)
            {
                // Replace settings values with stored in database
                if (haveSettingsObject.Settings != null)
                {
                    //Need clone settings entry because it may be shared for multiple instances
                    haveSettingsObject.Settings = haveSettingsObject.Settings.Select(x => (SettingEntry)x.Clone()).ToList();

                    foreach (var setting in haveSettingsObject.Settings)
                    {
                        var storedSetting = storedSettings.FirstOrDefault(x => String.Equals(x.Name, setting.Name, StringComparison.InvariantCultureIgnoreCase));
                        //First try to used stored object setting values
                        if (storedSetting != null)
                        {
                            setting.Value = storedSetting.Value;
                            setting.ArrayValues = storedSetting.ArrayValues;
                        }
                        else if (setting.Value == null && setting.ArrayValues == null)
                        {
                            //try to use global setting value
                            var globalSetting = GetSettingByName(setting.Name, companyId);
                            if (setting.IsArray)
                            {
                                setting.ArrayValues = globalSetting.ArrayValues ?? new[] { globalSetting.DefaultValue };
                            }
                            else
                            {
                                setting.Value = globalSetting.Value ?? globalSetting.DefaultValue;
                            }

                        }

                    }
                }
            }
        }

        public void SaveEntitySettingsValues(Entity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (entity.IsTransient())
                throw new ArgumentException("entity transistent");

            var objectType = entity.GetType().Name;

            var haveSettingsObjects = entity.GetFlatObjectsListWithInterface<IHaveSettings>();

            foreach (var haveSettingsObject in haveSettingsObjects)
            {
                var settings = new List<SettingEntry>();

                if (haveSettingsObject.Settings != null)
                {
                    //Save settings
                    foreach (var setting in haveSettingsObject.Settings)
                    {
                        setting.ObjectId = entity.Id;
                        setting.ObjectType = objectType;
                        settings.Add(setting);
                    }
                }
                SaveSettings(settings.ToArray());
            }
        }

        public void RemoveEntitySettings(Entity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (entity == null)
                throw new ArgumentNullException("entity transistent");

            var objectType = entity.GetType().Name;
            using (var repository = _repositoryFactory())
            {
                var settings = repository.Settings.Include(s => s.SettingValues)
                                                  .Where(x => x.ObjectId == entity.Id && x.ObjectType == objectType).ToList();
                foreach (var setting in settings)
                {
                    repository.Remove(setting);
                }
                repository.UnitOfWork.Commit();
            }

        }

        public SettingEntry[] GetModuleSettings(string moduleId, int? companyId)
        {
            var result = new List<SettingEntry>();

            var manifest = GetModuleManifestsWithSettings().FirstOrDefault(m => m.Id == moduleId);

            if (manifest != null && manifest.Settings != null && manifest.Settings.Any())
            {
                var settingEntities = GetAllEntities(companyId);
                //Load settings from requested module manifest with values from database
                foreach (var group in manifest.Settings)
                {
                    if (group.Settings != null)
                    {
                        foreach (var setting in group.Settings)
                        {
                            var dbSetting = settingEntities.FirstOrDefault(x => x.Name == setting.Name && x.ObjectId == null && x.CompanyId == companyId);

                            if (dbSetting == null && companyId != null)
                                dbSetting = settingEntities.FirstOrDefault(x => x.Name == setting.Name && x.ObjectId == null);

                            var settingEntry = setting.ToModel(dbSetting, group.Name);
                            settingEntry.ModuleId = moduleId;
                            result.Add(settingEntry);
                        }
                    }
                }
                //Try add runtime defined settings for requested module
                if (!string.IsNullOrEmpty(moduleId))
                {
                    List<SettingEntry> runtimeSettings;
                    if (_runtimeModuleSettingsMap.TryGetValue(moduleId, out runtimeSettings))
                    {
                        result.AddRange(runtimeSettings);
                    }
                }
            }
            return result.OrderBy(x => x.Name).ToArray();
        }

        /// <summary>
        /// Register module settings runtime
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="settings"></param>
        public void RegisterModuleSettings(string moduleId, params SettingEntry[] settings)
        {
            //check module exist
            if (!GetModules().Any(x => x.Id == moduleId))
            {
                throw new ArgumentException(moduleId + " not exist");
            }
            List<SettingEntry> moduleSettings;
            if (!_runtimeModuleSettingsMap.TryGetValue(moduleId, out moduleSettings))
            {
                moduleSettings = new List<SettingEntry>();
                _runtimeModuleSettingsMap[moduleId] = moduleSettings;
            }
            moduleSettings.AddRange(settings);
        }

        public void SaveSettings(SettingEntry[] settings)
        {
            if (settings != null && settings.Any())
            {
                var settingKeys = settings.Select(x => String.Join("-", x.Name, x.ObjectType, x.ObjectId, x.CompanyId)).Distinct().ToArray();

                using (var repository = _repositoryFactory())
                using (var changeTracker = new ObservableChangeTracker())
                {
                    var alreadyExistSettings = repository.Settings
                        .Include(s => s.SettingValues)
                        .Where(x => settingKeys.Contains(x.Name + "-" + x.ObjectType + "-" + x.ObjectId + "-" + x.CompanyId))
                        .ToList();

                    changeTracker.AddAction = x => repository.Add(x);
                    //Need for real remove object from nested collection (because EF default remove references only)
                    changeTracker.RemoveAction = x => repository.Remove(x);

                    var target = new { Settings = new ObservableCollection<SettingEntity>(alreadyExistSettings) };
                    var source = new { Settings = new ObservableCollection<SettingEntity>(settings.Select(x => x.ToEntity())) };

                    changeTracker.Attach(target);
                    var settingComparer = AnonymousComparer.Create((SettingEntity x) => String.Join("-", x.Name, x.ObjectType, x.ObjectId, x.CompanyId));
                    source.Settings.Patch(target.Settings, settingComparer, (sourceSetting, targetSetting) => sourceSetting.Patch(targetSetting));

                    repository.UnitOfWork.Commit();
                }

                var companyIds = settings.Select(x => x.CompanyId).Distinct().ToList();
                ClearCache(companyIds);
            }
        }

        public T[] GetArray<T>(string name, T[] defaultValue, int? companyId)
        {
            var result = defaultValue;

            var settingEntities = GetAllEntities(companyId);

            var repositorySetting = settingEntities.FirstOrDefault(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase) && s.CompanyId == companyId);

            if (repositorySetting == null && companyId != null)
                repositorySetting = settingEntities.FirstOrDefault(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase));

            if (repositorySetting != null)
            {
                result = GetOrderedSettingValues<T>(repositorySetting).ToArray();

                //result = repositorySetting.SettingValues
                //    .Select(v => v.RawValue())
                //    .Where(rv => rv != null)
                //    .Select(rv => (T)rv)
                //    .ToArray();
            }
            else
            {
                var manifestSetting = LoadSettingFromManifest(name);

                if (manifestSetting != null)
                {
                    if (manifestSetting.ArrayValues != null)
                    {
                        result = manifestSetting.ArrayValues
                            .Select(v => (T)manifestSetting.RawValue(v))
                            .ToArray();
                    }
                    else if (manifestSetting.DefaultValue != null)
                    {
                        result = new[] { (T)manifestSetting.RawDefaultValue() };
                    }
                }
            }

            return result;
        }

        IEnumerable<T> GetOrderedSettingValues<T>(SettingEntity settingEntity)
        {
            switch (settingEntity.SettingValueType)
            {
                case SettingValueEntity.TypeBoolean:
                    return settingEntity.SettingValues.Select(x => x.BooleanValue).OrderBy(x => x).Cast<T>();
                case SettingValueEntity.TypeDateTime:
                    return settingEntity.SettingValues.Select(x => x.DateTimeValue).Where(x => x.HasValue).OrderBy(x => x).Cast<T>();
                case SettingValueEntity.TypeDecimal:
                    return settingEntity.SettingValues.Select(x => x.DecimalValue).OrderBy(x => x).Cast<T>();
                case SettingValueEntity.TypeInteger:
                    return settingEntity.SettingValues.Select(x => x.IntegerValue).OrderBy(x => x).Cast<T>();
                case SettingValueEntity.TypeLongText:
                    return settingEntity.SettingValues.Select(x => x.LongTextValue).Where(x => x != null).OrderBy(x => x).Cast<T>();
                case SettingValueEntity.TypeShortText:
                    return settingEntity.SettingValues.Select(x => x.ShortTextValue).Where(x => x != null).OrderBy(x => x).Cast<T>();
                default:
                    return null;
            }
        }

        public T GetValue<T>(string name, T defaultValue, int? companyId = null)
        {
            var result = defaultValue;

            var values = GetArray(name, new[] { defaultValue }, companyId);

            if (values.Any())
            {
                result = values.First();
            }

            return result;
        }

        public void SetValue<T>(string name, T value)
        {
            var setting = name.ToModel(value);
            SaveSettings(new[] { setting });
        }

        #endregion


        private IEnumerable<ModuleManifest> GetModuleManifestsWithSettings()
        {
            return _manifestProvider.GetModuleManifests().Values
                .Union(_predefinedManifests)
                .Where(m => m.Settings != null && m.Settings.Any());
        }

        private ModuleSetting LoadSettingFromManifest(string name)
        {
            return GetAllManifestSettings().FirstOrDefault(s => s.Name == name);
        }

        private IEnumerable<ModuleSetting> GetAllManifestSettings()
        {
            return GetModuleManifestsWithSettings()
                .SelectMany(m => m.Settings)
                .SelectMany(g => g.Settings);
        }

        private List<SettingEntity> GetAllEntities(int? companyId)
        {
            var cacheKey = GetSettingCacheKey(companyId);
            var result = _cacheManager.Get(cacheKey, "PlatformRegion", () =>
                {
                    return LoadAllEntities(companyId);
                });
            return result;
        }

        private List<SettingEntity> LoadAllEntities(int? companyId)
        {
            using (var repository = _repositoryFactory())
            {
                return repository.Settings
                    .Where(x => x.ObjectType == null && x.ObjectId == null && (x.CompanyId == null || x.CompanyId == companyId))
                    .Include(s => s.SettingValues)
                    .ToList();
            }
        }

        private void ClearCache(IEnumerable<int?> companyIds)
        {
            foreach (var companyId in companyIds)
            {
                var cacheKey = GetSettingCacheKey(companyId);
                _cacheManager.Remove(cacheKey, "PlatformRegion");
            }
        }

        private static string GetSettingCacheKey(int? companyId)
        {
            return "CompanySettingsKey:" + (companyId ?? default(int));
        }
    }
}
