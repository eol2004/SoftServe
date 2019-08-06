using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ObjectBuilder2;

namespace VirtoCommerce.Platform.Core.Modularity
{
    public class ManifestModuleInfo : ModuleInfo
    {
        public string FullPhysicalPath { get; private set; }
        public ICollection<ManifestBundleItem> Styles { get; private set; }
        public ICollection<ManifestBundleItem> Scripts { get; private set; }

        public ManifestModuleInfo(ModuleManifest manifest, string fullPhysicalPath)
            : base(manifest.Id, manifest.ModuleType, (manifest.Dependencies ?? new ManifestDependency[0]).Select(d => d.Id).ToArray())
        {
            InitializationMode = InitializationMode.OnDemand;
            FullPhysicalPath = fullPhysicalPath;

            Title = manifest.Title;
            Description = manifest.Description;
            UseFullTypeNameInSwagger = manifest.UseFullTypeNameInSwagger;

            Styles = new List<ManifestBundleItem>();
            Scripts = new List<ManifestBundleItem>();

            if (manifest.Styles != null)
                manifest.Styles.ForEach(s => Styles.Add(s));

            if (manifest.Scripts != null)
                manifest.Scripts.ForEach(s => Scripts.Add(s));
        }

        public string Title { get; private set; }
        public string Description { get; set; }
        public bool UseFullTypeNameInSwagger { get; set; }
    }
}
