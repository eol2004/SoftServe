using VirtoCommerce.Platform.Core.Common;
namespace VirtoCommerce.Platform.Core.Settings
{
    public interface ISettingsManager
    {
        ModuleDescriptor[] GetModules();
        /// <summary>
        /// Deep load and populate settings values for entity and all nested objects 
        /// </summary>
        void LoadEntitySettingsValues(Entity entity, int? companyId);
        /// <summary>
        /// Deep save entity and all nested objects settings values
        /// </summary>
        /// <param name="entity"></param>
        void SaveEntitySettingsValues(Entity entity);
        /// <summary>
        /// Deep remove entity and all nested objects settings values
        /// </summary>
        /// <param name="entity"></param>
		void RemoveEntitySettings(Entity entity);
        SettingEntry GetSettingByName(string name, int? companyId = null);
        SettingEntry[] GetModuleSettings(string moduleId, int? companyId);
		void SaveSettings(SettingEntry[] settings);
        /// <summary>
        /// Used to runtime settings registration
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="settings"></param>
        void RegisterModuleSettings(string moduleId, params SettingEntry[] settings);

        T GetValue<T>(string name, T defaultValue, int? companyId = null);
        T[] GetArray<T>(string name, T[] defaultValue, int? companyId);
        void SetValue<T>(string name, T value);
    }
}
