using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

// TO DO:
// - Make async

namespace NorskaLib.Storage
{
    public class BinaryLocalStorage : LocalStorage
    {
        public static BinaryLocalStorage Instance { get; private set; }

        public BinaryLocalStorage()
        {
            Instance = this;
        }

        private const string FileFormat = "bin";

        private Dictionary<string, string> filesPathes = new Dictionary<string, string>(3);
        private string GetPath(string filename)
        {
            if (filesPathes.TryGetValue(filename, out var path))
                return path;
            else
            {
                path = $"{Application.persistentDataPath}/{filename}.{FileFormat}";
                filesPathes.Add(filename, path);
                return path;
            }
        }

        private string activeSlot;
        public override string ActiveSlot => activeSlot;

        private void LoadModules(string filename,IStorageModule[] collection)
        {
            var dataExist = TryRead(filename, out var modulesData);
            var wantUpdateFile = false;

            foreach (var module in collection)
                if (dataExist)
                {
                    if (modulesData.TryGetValue(module.GetType(), out var data))
                    {
                        module.SetSerializedState(data);
                    }
                    else
                    {
                        module.CreateDefaultState();
                        wantUpdateFile |= true;
                    }
                }
                else
                {
                    module.CreateDefaultState();
                    wantUpdateFile |= true;
                }

            if (wantUpdateFile)
                Write(filename, collection);
        }

        private void Write(string filename, IEnumerable<IStorageModule> modules)
        {
            var path = GetPath(filename);

            var formatter = new BinaryFormatter();
            var stream = new FileStream(path, FileMode.Create);

            var modulesData = modules.ToDictionary(m => m.GetType(), m => m.GetSerializedState());
            formatter.Serialize(stream, modulesData);
            stream.Close();
        }

        private void TryDelete(string filename)
        {
            var path = GetPath(filename);

            if (!File.Exists(path))
                return;

            File.Delete(path);
        }

        private bool TryRead(string filename, out Dictionary<Type, byte[]> data)
        {
            var path = GetPath(filename);

            if (!File.Exists(path))
            {
                data = null;
                return false;
            }

            var formatter = new BinaryFormatter();
            var stream = new FileStream(path, FileMode.Open);

            data = formatter.Deserialize(stream) as Dictionary<Type, byte[]>;
            stream.Close();

            return true;
        }

        public override void LoadShared()
        {
            if (!IsInitialized)
            {
                Debug.LogError($"Storage is not initialized!");
                return;
            }

            LoadModules(SharedFilename, modulesShared);
        }

        public override void LoadSlot(string name)
        {
            if (!IsInitialized)
            {
                Debug.LogError($"Storage is not initialized!");
                return;
            }

            LoadModules(name, modulesSlot);

            activeSlot = name;
        }

        public override void SaveShared()
        {
            if (!IsInitialized)
            {
                Debug.LogError($"Storage is not initialized!");
                return;
            }

            Write(SharedFilename, modulesShared);
        }

        public override void SaveSlot(string name)
        {
            if (!IsInitialized)
            {
                Debug.LogError($"Storage is not initialized!");
                return;
            }

            Write(name, modulesSlot);

            onSlotChanged?.Invoke(name);
        }

        public override void DeleteSlot(string name)
        {
            TryDelete(name);

            onSlotChanged?.Invoke(name);
        }

        public static FileData[] GetExistingFiles(string namePattern = null)
        {
            //Debug.Log($"Detecting files at {Application.persistentDataPath}...");

            var searchPattern = string.IsNullOrEmpty(namePattern)
                ? $"*.{FileFormat}"
                : $"{namePattern}*.{FileFormat}";

            //Debug.Log($"Search pattern is {searchPattern}...");

            var result = Directory
                .GetFiles(Application.persistentDataPath, searchPattern, SearchOption.TopDirectoryOnly)
                .Select(path => new FileData()
                    {
                        name = Path.GetFileNameWithoutExtension(path),
                        dateModified = File.GetLastWriteTime(path),
                    })
                .OrderByDescending(d => d.dateModified)
                .ToArray();

            //if (result.Length == 0)
            //    Debug.Log($"No files detected...");
            //else
            //{
            //    Debug.Log($"Detected save files:");
            //    foreach (var data in result)
            //        Debug.Log($"-> '{data.name}'");
            //}

            return result;
        }
    }
}