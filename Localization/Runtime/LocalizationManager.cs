using System;
using System.Globalization;
using UnityEngine;

namespace NorskaLib.Localization
{
    public partial class LocalizationManager
    {
        private readonly ILocalizationContainer container;
        private const string SerializationKey = "Localization/SelectedLanguage";

        public Action<Language> onLanguageChanged;

        public Language CurrentLanguage 
        { get; private set; } = Language.UNIDENTIFIED;

        public LocalizationManager(ILocalizationContainer container)
        {
            this.container = container;

            var language = (Language) PlayerPrefs.GetInt(SerializationKey, -1);
            if (language == Language.UNIDENTIFIED)
                language = GetSystemLanguage();

            //Debug.Log($"Detected system UI language is {CurrentLanguage}");

            SwitchLanguage(language);
        }

        // TO DO:
        // More languages
        private Language GetSystemLanguage()
        {
            var ci = CultureInfo.CurrentUICulture;
            switch (ci.CompareInfo.Name)
            {
                case LanguagesCodes.RUS:
                    return Language.RUS;

                case LanguagesCodes.ESP:
                    return Language.ESP;

                default:
                    return Language.ENG;
            }
        }

        public void SwitchLanguage(Language language)
        {
            if (CurrentLanguage == language)
                return;

            CurrentLanguage = language;
            PlayerPrefs.SetInt(SerializationKey, (int)language);
            onLanguageChanged?.Invoke(language);
        }

        public string GetText(string key)
        {
            if (!container.TryGetData(key, out var data))
                return key;

            return data.GetText(CurrentLanguage);
        }
    }
}