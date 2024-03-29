﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Data.Common;
using VirtoCommerce.Platform.Data.DynamicProperties.Converters;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Model;
using VirtoCommerce.Platform.Data.Repositories;

namespace VirtoCommerce.Platform.Data.DynamicProperties
{
    public class DynamicPropertyService : ServiceBase, IDynamicPropertyService
    {
        private List<string> _availableTypeNames = new List<string>();
        private readonly Func<IPlatformRepository> _repositoryFactory;

        public DynamicPropertyService(Func<IPlatformRepository> repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
            _availableTypeNames.AddRange(LoadTypesFromReflection());
        }

        private IEnumerable<string> LoadTypesFromReflection()
        {
            var typeName = typeof(IHasDynamicProperties).Name;
            return AppDomain.CurrentDomain.GetAssemblies()
                   .SelectMany(a => a.GetLoadableTypes())
                   .Where(t => t.IsClass && t.IsPublic && !t.IsAbstract && t.GetInterface(typeName) != null)
                   .Select(GetObjectTypeName);
        }

        #region IDynamicPropertyService Members

        public void RegisterType(string typeName)
        {
            if (!_availableTypeNames.Contains(typeName, StringComparer.OrdinalIgnoreCase))
            {
                _availableTypeNames.Add(typeName);
            }
        }

        public string[] GetAvailableObjectTypeNames()
        {
            return _availableTypeNames.ToArray();
        }

        public string GetObjectTypeName(Type type)
        {
            return type.FullName;
        }

        public DynamicProperty[] GetProperties(string objectType)
        {
            if (objectType == null)
                throw new ArgumentNullException("objectType");

            var result = new List<DynamicProperty>();

            using (var repository = _repositoryFactory())
            {
                var properties = repository.GetDynamicPropertiesForType(objectType);
                result.AddRange(properties.Select(p => p.ToModel()));
            }
            return result.ToArray();
        }

        public DynamicProperty[] SaveProperties(DynamicProperty[] properties)
        {
            if (properties == null)
                throw new ArgumentNullException("properties");

            using (var repository = _repositoryFactory())
            using (var changeTracker = GetChangeTracker(repository))
            {
                var sourceProperties = properties.Select(x => x.ToEntity()).ToList();
                var targetProperties = repository.GetDynamicPropertiesByIds(properties.Select(x => x.Id).ToArray()).ToList();
                sourceProperties.CompareTo(targetProperties, EqualityComparer<DynamicPropertyEntity>.Default, (state, source, target) =>
                    {
                        if (state == EntryState.Modified)
                        {
                            changeTracker.Attach(target);
                            source.Patch(target);
                        }
                        else if (state == EntryState.Added)
                        {
                            repository.Add(source);
                        }

                    });
                repository.UnitOfWork.Commit();

                var result = repository.GetDynamicPropertiesByIds(sourceProperties.Select(p => p.Id).ToArray())
                    .Select(p => p.ToModel())
                    .ToArray();
                return result;
            }
        }

        public void DeleteProperties(string[] propertyIds)
        {
            if (propertyIds == null)
                throw new ArgumentNullException("propertyIds");

            using (var repository = _repositoryFactory())
            {
                var properties = repository.DynamicProperties
                    .Where(p => propertyIds.Contains(p.Id))
                    .ToList();

                foreach (var property in properties)
                {
                    repository.Remove(property);
                }

                repository.UnitOfWork.Commit();
            }
        }


        public DynamicPropertyDictionaryItem[] GetDictionaryItems(string propertyId)
        {
            if (propertyId == null)
                throw new ArgumentNullException("propertyId");

            using (var repository = _repositoryFactory())
            {
                var items = repository.GetDynamicPropertyDictionaryItems(propertyId);
                var result = items.OrderBy(i => i.Name).Select(i => i.ToModel()).ToArray();
                return result;
            }
        }

        public void SaveDictionaryItems(string propertyId, DynamicPropertyDictionaryItem[] items)
        {
            if (propertyId == null)
                throw new ArgumentNullException("propertyId");
            if (items == null)
                throw new ArgumentNullException("items");

            using (var repository = _repositoryFactory())
            using (var changeTracker = GetChangeTracker(repository))
            {
                var property = repository.GetDynamicPropertiesByIds(new[] { propertyId }).First();
                var sourceDicItems = items.Select(x => x.ToEntity(property)).ToList();
                var targetDicItems = repository.GetDynamicPropertyDictionaryItems(propertyId).ToList();

                sourceDicItems.CompareTo(targetDicItems, EqualityComparer<DynamicPropertyDictionaryItemEntity>.Default, (state, source, target) =>
                {
                    if (state == EntryState.Modified)
                    {
                        changeTracker.Attach(target);
                        source.Patch(target);
                    }
                    else if (state == EntryState.Added)
                    {
                        repository.Add(source);
                    }

                });
                repository.UnitOfWork.Commit();
            }
        }

        public void DeleteDictionaryItems(string[] itemIds)
        {
            if (itemIds == null)
                throw new ArgumentNullException("itemIds");

            using (var repository = _repositoryFactory())
            {
                var items = repository.DynamicPropertyDictionaryItems
                    .Where(v => itemIds.Contains(v.Id))
                    .ToList();

                foreach (var item in items)
                {
                    repository.Remove(item);
                }

                repository.UnitOfWork.Commit();
            }
        }

        public void LoadDynamicPropertyValues(IHasDynamicProperties owner)
        {
            LoadDynamicPropertyValues(new[] { owner });
        }

        public void LoadDynamicPropertyValues(IHasDynamicPropertiesGuid owner)
        {
            LoadDynamicPropertyValues(new[] { owner });
        }

        public void LoadDynamicPropertyValues(params IHasDynamicProperties[] owners)
        {
            LoadDynamicPropertyValuesInternal(owners, (x) => x.Id);
        }

        public void LoadDynamicPropertyValues(params IHasDynamicPropertiesGuid[] owners)
        {
            LoadDynamicPropertyValuesInternal(owners, (x) => x.Guid.ToString());
        }

        private void LoadDynamicPropertyValuesInternal<TItem>(TItem[] owners, Func<TItem, string> idSelector)
            where TItem : IHasDynamicPropertiesBase
        {
            if (owners == null)
            {
                throw new ArgumentNullException("owners");
            }

            var propOwners = owners.SelectMany(x => x.GetFlatObjectsListWithInterface<TItem>());
            using (var repository = _repositoryFactory())
            {
                var objectTypeNames = propOwners.Select(x => GetObjectTypeName(x)).Distinct().ToArray();
                var objectIds = propOwners.Select(x => idSelector(x)).Distinct().ToArray();

                var dbDynamicPorps = repository.GetObjectDynamicProperties(objectTypeNames, objectIds);
                foreach (var propOwner in propOwners)
                {
                    var objectType = GetObjectTypeName(propOwner);
                    propOwner.DynamicProperties = dbDynamicPorps.Where(x => x.ObjectType == objectType).Select(p => p.ToDynamicObjectProperty(idSelector(propOwner))).ToList();
                    propOwner.ObjectType = GetObjectTypeName(propOwner);
                }
            }
        }

        public void SaveDynamicPropertyValues(IHasDynamicProperties owner)
        {
            SaveDynamicPropertyValuesInternal(owner, (x) => x.Id);
        }

        public void SaveDynamicPropertyValues(IHasDynamicPropertiesGuid owner)
        {
            SaveDynamicPropertyValuesInternal(owner, (x) => x.Guid.ToString());
        }

        private void SaveDynamicPropertyValuesInternal<TItem>(TItem owner, Func<TItem, string> idSelector)
            where TItem : IHasDynamicPropertiesBase
        {
            if (owner == null)
            {
                throw new ArgumentNullException("owner");
            }
            //Because one DynamicPropertyEntity may update for multiple object in same time
            //need create fresh repository for each object to prevent collisions and overrides property values
            var objectsWithDynamicProperties = owner.GetFlatObjectsListWithInterface<TItem>();
            foreach (var objectWithDynamicProperties in objectsWithDynamicProperties)
            {
                using (var repository = _repositoryFactory())
                using (var changeTracker = GetChangeTracker(repository))
                {
                    var objectId = idSelector(objectWithDynamicProperties);
                    if (objectId != null)
                    {
                        if (objectWithDynamicProperties.DynamicProperties != null && objectsWithDynamicProperties.Any())
                        {
                            var objectType = GetObjectTypeName(objectWithDynamicProperties);

                            var sourceCollection = objectWithDynamicProperties.DynamicProperties.Select(x => x.ToEntity(objectId, objectType));
                            var targetCollection = repository.GetObjectDynamicProperties(new[] { objectType }, new[] { objectId });

                            var target = new { Properties = new ObservableCollection<DynamicPropertyEntity>(targetCollection) };
                            var source = new { Properties = new ObservableCollection<DynamicPropertyEntity>(sourceCollection) };

                            //When creating DynamicProperty manually, many properties remain unfilled (except Name, ValueType and ObjectValues).
                            //We have to set them with data from the repository.
                            var transistentProperties = source.Properties.Where(x => x.IsTransient());
                            if (transistentProperties.Any())
                            {
                                var allTypeProperties = repository.GetDynamicPropertiesForType(objectType);
                                foreach (var transistentPropery in transistentProperties)
                                {
                                    var property = allTypeProperties.FirstOrDefault(x => String.Equals(x.Name, transistentPropery.Name, StringComparison.InvariantCultureIgnoreCase));
                                    if (property != null)
                                    {
                                        transistentPropery.Id = property.Id;
                                        transistentPropery.ObjectType = property.ObjectType;
                                        transistentPropery.IsArray = property.IsArray;
                                        transistentPropery.IsRequired = property.IsRequired;
                                        transistentPropery.ValueType = property.ValueType;
                                    }
                                }
                            }
                            changeTracker.Attach(target);
                            foreach (var sourceProperty in source.Properties)
                            {
                                var targetProperty = target.Properties.FirstOrDefault(x => x.Id == sourceProperty.Id);
                                if (targetProperty != null)
                                {
                                    if (!sourceProperty.ObjectValues.IsNullCollection())
                                    {
                                        sourceProperty.ObjectValues.Patch(targetProperty.ObjectValues, new DynamicPropertyObjectValueComparer(), (sourceValue, targetValue) => sourceValue.Patch(targetValue));
                                    }
                                }
                            }
                        }
                    }

                    repository.UnitOfWork.Commit();
                }
            }
        }

        public void DeleteDynamicPropertyValues(IHasDynamicProperties owner)
        {
            var objectsWithDynamicProperties = owner.GetFlatObjectsListWithInterface<IHasDynamicProperties>();

            using (var repository = _repositoryFactory())
            {
                foreach (var objectWithDynamicProperties in objectsWithDynamicProperties.Where(x => x.Id != null))
                {
                    var objectType = GetObjectTypeName(objectWithDynamicProperties);
                    var objectId = objectWithDynamicProperties.Id;

                    var values = repository.DynamicPropertyObjectValues
                        .Where(v => v.ObjectType == objectType && v.ObjectId == objectId)
                        .ToList();

                    foreach (var value in values)
                    {
                        repository.Remove(value);
                    }
                }

                repository.UnitOfWork.Commit();
            }
        }

        #endregion

        private string GetObjectTypeName(object obj)
        {
            return GetObjectTypeName(obj.GetType());
        }


    }
}
