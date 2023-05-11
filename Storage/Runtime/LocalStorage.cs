using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NorskaLib.DI;

namespace NorskaLib.Storage
{
    public abstract class LocalStorage
    {
        protected string SharedFilename { get; private set; }

        protected IStorageModule[] modulesShared;
        protected IStorageModule[] modulesSlot;

        public abstract string ActiveSlot { get; }

        public Action<string> onSlotChanged;

        public bool IsInitialized { get; private set; }

        public void Initialize(IEnumerable<Type> allModulesTypes, string sharedFilename)
        {
            bool SharedPredicate(Type type)
                => Attribute.IsDefined(type, typeof(SharedStorageModuleAttribute));

            bool SlotPredicate(Type type)
                => !Attribute.IsDefined(type, typeof(SharedStorageModuleAttribute));

            void RegisterModules(ref IStorageModule[] collection, Func<Type, bool> predicate)
            {
                var types = allModulesTypes
                    .Where(predicate)
                    .ToArray();
                collection = new IStorageModule[types.Length];
                for (int i = 0; i < collection.Length; i++)
                {
                    var moduleType = types[i];
                    var moduleInstance = Activator.CreateInstance(moduleType) as IStorageModule;
                    DependenciesContainer.Instance.RegisterInstance(moduleInstance, moduleType);

                    collection[i] = moduleInstance;
                }
            }

            SharedFilename = sharedFilename;

            RegisterModules(ref modulesShared, SharedPredicate);
            RegisterModules(ref modulesSlot, SlotPredicate);

            IsInitialized = true;
        }

        public abstract void LoadShared();

        public abstract void LoadSlot(string name);

        public abstract void SaveShared();

        public abstract void SaveSlot(string name);

        public abstract void DeleteSlot(string name);

        /// <summary>
        /// Saves current loaded slot.
        /// </summary>
        public void SaveSlot()
        {
            if (string.IsNullOrEmpty(ActiveSlot))
            {
                Debug.LogError($"No slot loaded!");
                return;
            }

            SaveSlot(ActiveSlot);
        }
    }
}