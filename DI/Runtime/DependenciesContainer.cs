using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace NorskaLib.DI
{
    public class DependenciesContainer
    {
        #region Static

        public static DependenciesContainer Instance { get; private set; }

        public static DependenciesContainer Initialize()
        {
            Instance = new DependenciesContainer();
            return Instance;
        }

        #endregion

        private readonly Dictionary<Type, object> instances = new();

        public void RegisterInstance<T>(T instance) where T : class
        {
            var type = typeof(T);
            RegisterInstance(instance, type);
        }

        public void RegisterInstance(object instance, Type type)
        {
            if (!instances.ContainsKey(type))
                instances.Add(type, instance);
            else
                instances[type] = instance;
        }

        public void RegisterCollectionItem<T>(T instance)
        {
            var collection = default(List<T>);
            if (!TryResolveInternal(typeof(List<T>), out var c))
            {
                collection = new List<T>();
                RegisterInstance(collection);
            }
            else
            {
                collection = c as List<T>;
            }

            if (collection.Contains(instance))
            {
                Debug.LogWarning($"Skip registring collection instance of type '{typeof(T).Name}' as it is already present in the collection.");
                return;
            }

            collection.Add(instance);
        }

        public void UnregisterInstance<T>(T instance) where T : class
        {
            var type = typeof(T);
            if (!instances.TryGetValue(type, out var registredInstance) || registredInstance != instance)
                return;
            else
                instances[type] = null;
        }

        public void UnregisterCollectionItem<T>(T instance)
        {
            var collection = Resolve<List<T>>();
            if (collection == null)
            {
                Debug.LogWarning($"Missing collection instance of type '{typeof(List<T>).Name}'");
                return;
            }

            collection.Remove(instance);
        }

        public T Resolve<T>() where T : class
        {
            if (!TryResolveInternal(typeof(T), out var result))
                Debug.LogWarning($"Couldn't resolve dependency for type {typeof(T).Name}");

            return result as T;
        }

        public object Resolve(Type type)
        {
            if (!TryResolveInternal(type, out var result))
                Debug.LogWarning($"Couldn't resolve dependency for type {type}");

            return result;
        }

        private bool TryResolveInternal(Type type, out object result)
        {
            if (instances.TryGetValue(type, out var instance))
            {
                result = instance;
                return true;
            }

            foreach (var registredType in instances.Keys)
                if (registredType.IsAssignableFrom(type))
                {
                    result = instances[registredType];
                    return true;
                }

            result = null;
            return false;
        }

        public List<T> ResolveCollection<T>()
        {
            return Resolve<List<T>>();
        }

        public void BuildUp(object target)
        {
            var targetType = target.GetType();
            var dependenciesFieldsInfos = targetType
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(fi => Attribute.IsDefined(fi, typeof(DependencyAttribute)));

            foreach (var dependency in dependenciesFieldsInfos)
            {
                var value = Resolve(dependency.FieldType);
                dependency.SetValue(target, value);
            }
        }

        public void Log()
        {
            Debug.Log($"Total dependencies registred:");
            foreach (var pair in instances)
                Debug.Log($"-> '{pair.Key.Name}'");
        }
    }
}