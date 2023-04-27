namespace NorskaLib.Pools
{
    /// <summary>
    /// Optional interface that pooled objects can implement to add some specific logic (for example, if your object can be enabled/disabled without beign returned to the pool).
    /// </summary>
    public interface IPoolable
    {
        /// <summary>
        /// Called by the Pool each time object is taken from it (or instantiated for the first time).
        /// </summary>
        public void OnAllocated();

        /// <summary>
        /// Called by the Pool each time object is returned to it (before it is destroyed if that is the case).
        /// </summary>
        public void OnDeallocated();
    }
}