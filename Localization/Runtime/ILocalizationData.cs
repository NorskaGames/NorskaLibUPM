using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NorskaLib.Localization
{ 
    public interface ILocalizationData
    {
        public string GetText(Language language);
    }
}