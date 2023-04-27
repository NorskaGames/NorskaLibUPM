//using System.Linq;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using NorskaLib.Utilities;

//namespace NorskaLib.UI
//{
//    public class FloatingTextPool : MonoBehaviour
//    {
//        [SerializeField] FloatingText[] instances;
//        private List<FloatingText> instancesOccupied;

//        public float UpBorderAddOffset;
    
//        protected RectTransform RectTransform { get; private set; }
    
//        void Awake()
//        {
//            RectTransform = transform as RectTransform;
//            instancesOccupied = new List<FloatingText>(instances.Length);

//            foreach (var ft in instances)
//                ft.onSequenceFinished += OnInstanceFinishesAnimating;
//        }

//        void OnDestroy()
//        {
//            foreach (var ft in instances)
//                ft.onSequenceFinished -= OnInstanceFinishesAnimating;
//        }

//        /// <summary>
//        /// Returns TRUE is floating text instance is animation at the moment.
//        /// </summary>
//        /// <returns></returns>
//        public bool IsAnimating(FloatingText text)
//        {
//            return instancesOccupied.Contains(text);
//        }

//        public FloatingText Show(string message, Vector3 worldPos, Camera camera)
//        {
//            var rectPos = Utils.WorldToRectPosition(camera, RectTransform, worldPos);
        
//            var instance = instances.FirstOrDefault(i => !instancesOccupied.Contains(i));
//            if (instance == null)
//            {
//                instance = instancesOccupied[0];
//                instancesOccupied.RemoveAt(0);
//            }

//            var x = rectPos.x;
//            var y = rectPos.y;

//            var widgetRect = instance.Rect.rect;
//            var mapRect = this.RectTransform.rect;

//            x = Mathf.Clamp(x, widgetRect.width / 2f, mapRect.width - widgetRect.width / 2f);
//            y = Mathf.Clamp(y, 0, mapRect.height - widgetRect.height / 2f - UpBorderAddOffset);

//            var pos = new Vector2(x, y);
        
//            instance.Rect.anchoredPosition = pos;
//            instance.Text = message;
//            instance.Animate();
//            instancesOccupied.Add(instance);

//            return instance;
//        }

//        /// <summary>
//        /// Instantly hides text object, stops its animation and adds it back to pool.
//        /// </summary>
//        /// <param name="text"></param>
//        public void Release(FloatingText text)
//        {
//            text.Break();

//            instancesOccupied.Remove(text);
//        }

//        void OnInstanceFinishesAnimating(FloatingText text)
//        {
//            instancesOccupied.Remove(text);
//        }
//    }
//}
