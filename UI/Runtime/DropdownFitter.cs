using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Dropdown = TMPro.TMP_Dropdown;

[RequireComponent(typeof(Dropdown))]
public class DropdownFitter : MonoBehaviour
{
    [SerializeField] int optionsLimit = 3;
    [SerializeField] ContentSizeFitter sizeFitter;
    [SerializeField] LayoutGroup layoutGroup;
    private Dropdown dropdown;

    void Awake()
    {
        dropdown = gameObject.GetComponent<Dropdown>();
    }

    public void UpdateLayout()
    {
        sizeFitter.enabled = dropdown.options.Count <= optionsLimit;
        layoutGroup.enabled = dropdown.options.Count <= optionsLimit;
    }
}
