using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NorskaLib.Pools
{
    public abstract class GenericComponentPool<C> : ComponentPool<C> where C : Component
    {
        public C prefab;

        protected override C Instantiate()
        {
            return Instantiate(prefab);
        }

        #region Editor
#if UNITY_EDITOR

        [SerializeField] bool logWarning = false;

        protected override void Awake()
        {
            base.Awake();

            if (logWarning && prewarmedInstances.Length > capacity)
                Debug.Log($"Prewarmed instances count in {this.name} ({this.GetType().Name}) is bigger than the pool's capacity. Some of these instances will never be used.");
        }

#endif
        #endregion
    }

    public abstract class ComponentPool<C> : MonoBehaviour where C : Component
    {
        [Tooltip("Preffered amount of instances in the pool. " +
            "If an instances gets deacllocated and pool already has given amount of instances, " +
            "it will be destroyed, rather than disabled.")]
        public int capacity = 8;

        [Space]

        [Tooltip("Transform that serves as a parent for deallocated instances")]
        public Transform container;

        [Tooltip("If set, game objects will be activated/deactivated upon allocation/deallocation.")]
        public bool disablePooled = true;

        [Tooltip("Instances that must be loaded with scene and moved into the pool.")]
        [SerializeField] protected C[] prewarmedInstances;

        private Stack<C> stack;

        private bool isInitialized;
        protected virtual void Initialize()
        {
            stack ??= new Stack<C>(capacity);
        }

        protected abstract C Instantiate();

        public C Allocate()
        {
            stack ??= new Stack<C>(capacity);

            var instance = stack.Count > 0
                ? stack.Pop()
                : Instantiate();

            if (disablePooled)
                instance.gameObject.SetActive(true);

            if (instance is IPoolable poolable)
                poolable.OnAllocated();

            return instance;
        }

        public C Allocate(Transform parent, bool worldPositionStays = false)
        {
            var instance = Allocate();
            instance.transform.SetParent(parent, worldPositionStays);

            return instance;
        }

        public C Allocate(Vector3 position, Quaternion rotation)
        {
            var instance = Allocate();
            instance.transform.SetPositionAndRotation(position, rotation);

            return instance;
        }

        public void Deallocate(C instance)
        {
            stack ??= new Stack<C>(capacity);

            if (instance is IPoolable poolable)
                poolable.OnDeallocated();

            if (stack.Count < capacity)
            {
                stack.Push(instance);
                instance.transform.SetParent(container);

                if (disablePooled)
                    instance.gameObject.SetActive(false);
            }
            else
            {
                Destroy(instance.gameObject);
            }
        }

        protected virtual void Awake()
        {
            stack ??= new Stack<C>(capacity);

            if (prewarmedInstances == null || prewarmedInstances.Length == 0)
                return;

            if (disablePooled)
                foreach (var instance in prewarmedInstances)
                    instance.gameObject.SetActive(false);

            var prewarmedTake = prewarmedInstances.Take(capacity);
            foreach (var instance in prewarmedTake)
                stack.Push(instance);
        }
    }
}