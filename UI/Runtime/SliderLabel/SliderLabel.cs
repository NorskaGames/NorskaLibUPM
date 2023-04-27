using UnityEngine;
using UnityEngine.UI;

namespace NorskaLib.UI
{
    public class SliderLabel : MonoBehaviour
    {
        [SerializeField] Slider slider;

        [Space]

        [SerializeField] ValueLabel crntValueLabel;
        [SerializeField] ValueLabel minValueLabel;
        [SerializeField] ValueLabel maxValueLabel;

        #region MonoBehaviour

        void OnEnable()
        {
            if (minValueLabel != null)
                minValueLabel.DisplayImmediate(slider.minValue);
            if (maxValueLabel != null)
                maxValueLabel.DisplayImmediate(slider.maxValue);

            OnSliderValueChanged(slider.value);
            slider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        void OnDisable()
        {
            slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }

        #endregion

        void OnSliderValueChanged(float value)
        {
            if (crntValueLabel != null)
                crntValueLabel.DisplayImmediate(value);
        }
    } 
}