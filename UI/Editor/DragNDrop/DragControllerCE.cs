using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;

[CustomEditor(typeof(DragController))]
public class DragControllerCE : OdinEditor
{
    private void OnSceneGUI()
    {
        Repaint();
    }
}