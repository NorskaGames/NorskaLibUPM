using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NorskaLib.Extensions
{
    public static class GameObjectExtensions
    {
        // TO DO:
        // Use breadth-first search for performance
        public static void SetLayerDownHierarchy(this GameObject instance, int layer)
        {
            instance.layer = layer;

            foreach (Transform child in instance.transform)
                SetLayerDownHierarchy(child.gameObject, layer);
        }
    }
}