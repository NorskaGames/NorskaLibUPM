using NorskaLib.DI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NorskaLib.Localization
{
	public abstract class Localizer : MonoBehaviour
	{
        #region Dependencies

        private DependenciesContainer DependenciesContainer => DependenciesContainer.Instance;
        [Dependency] protected LocalizationManager LocalizationManager;

        #endregion

        [SerializeField] string key;

        private Language language = Language.UNIDENTIFIED;

        protected virtual void Awake()
        {
            LocalizationManager = DependenciesContainer.Resolve<LocalizationManager>();
        }

        protected virtual void OnEnable()
        {
            if (language != LocalizationManager.CurrentLanguage)
                OnLocalizationChanged(LocalizationManager.CurrentLanguage);

            LocalizationManager.onLanguageChanged += OnLocalizationChanged;
        }

        protected virtual void OnDisable()
        {
            LocalizationManager.onLanguageChanged -= OnLocalizationChanged;
        }

        void OnLocalizationChanged(Language language)
        {
            SetText(LocalizationManager.GetText(key));

            this.language = language;
        }

        protected abstract void SetText(string localizedText);
    }
}