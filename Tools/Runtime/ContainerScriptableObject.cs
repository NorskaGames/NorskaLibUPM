using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NorskaLib.Tools
{
    public abstract class ContainerScriptableObject<T> : ScriptableObject, IEnumerable<T> where T : class
    {
        [SerializeField] protected T[] collection;

        #region IEnumerable

        public T this[int index] => collection[index];

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)collection).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        #endregion

        public virtual T GetDefault() => default;

        public T GetMatchingOrDefault(Func<T, bool> predicate)
        {
            foreach (var item in collection)
                if (predicate(item))
                    return item;

            Debug.LogWarning($"No '{typeof(T).Name}' is '{this.name}' matching predicate. Returning default.");
            return GetDefault();
        }
    }
}