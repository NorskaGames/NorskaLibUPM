using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Label = TMPro.TextMeshProUGUI;

namespace NorskaLib.Localization
{
    public class TMProLocalizer : Localizer
    {
        [SerializeField] Label label;

        protected override void SetText(string localizedText)
        {
            label.text = localizedText;
        }
    }
}