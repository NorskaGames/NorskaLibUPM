using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NorskaLib.Storage
{
    public interface IStorageModule 
    {
        public bool HasState { get; }

        public byte[] GetSerializedState();

        public void SetSerializedState(byte[] data);

        public void CreateDefaultState();
    }

    public abstract class StorageModule<T> : IStorageModule where T : class, new()
    {
        protected T State { get; private set; }

        public bool HasState => State != null;

        public byte[] GetSerializedState()
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, State);
                return stream.ToArray();
            }
        }

        public void SetSerializedState(byte[] data)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream(data))
            {
                State = formatter.Deserialize(stream) as T;
            }
        }

        /// <summary>
        ///  Called when there is no module state found 
        ///  (usually, when app is launched for the first time or
        ///  saves has been deleted by the user).
        /// </summary>
        public virtual void CreateDefaultState()
        {
            State = new T();
        }
    }

    /// <summary>
    /// Use this attribute for storage modules which store data
    /// that exist in a single instance (e. g. user account data or analytics).
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
    public class SharedStorageModuleAttribute : System.Attribute
    {

    }
}