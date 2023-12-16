using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NorskaLib.Extensions
{
    public abstract class ContainerScriptableObject<T> : ScriptableObject, IEnumerable<T> where T : class
    {
        //private const string ModificationExceptionText = "ContainerScriptableObject<> internal collection is immutable by design!";

        [SerializeField] protected T[] collection;

        #region IEnumerable

        public T this[int index]
        {
            get => collection[index];

            set => collection[index] = value; //throw new System.NotSupportedException(ModificationExceptionText);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)collection).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        #endregion

        public int Count => collection.Length;

        public int IndexOf(T item)
        {
            for (int i = 0; i < collection.Length; i++)
                if (item == collection[i])
                    return i;

            return -1;
        }

        public bool Contains(T item)
        {
            for (int i = 0; i < collection.Length; i++)
                if (item == collection[i])
                    return true;

            return false;
        }

        public virtual T GetDefault() => default;

        public T GetMatchingOrDefault(Func<T, bool> predicate)
        {
            foreach (var item in collection)
                if (predicate(item))
                    return item;

            Debug.LogWarning($"No '{typeof(T).Name}' in '{this.name}({this.GetType().Name})' matching predicate. Returning default.");
            return GetDefault();
        }
    }
}