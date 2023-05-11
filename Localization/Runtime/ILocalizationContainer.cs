using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NorskaLib.Localization
{
    public interface ILocalizationContainer
    {
        public bool TryGetData(string key, out ILocalizationData data);
    }
}