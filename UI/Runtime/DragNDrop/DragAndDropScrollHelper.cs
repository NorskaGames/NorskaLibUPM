using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using NorskaLib.Extensions;

#region Editor
#if UNITY_EDITOR

using UnityEditor;
using Sirenix.OdinInspector.Editor;

#endif
#endregion

namespace NorskaLib.UI.DragAndDrop
{
    [RequireComponent(typeof(RectTransform))]
	public class DragAndDropScrollHelper : MonoBehaviour
	{
		public ScrollRect scroll;

		public DragAndDropDisplay display;

		public Axis axis = Axis.Vertical;
        public enum Axis
		{
			Vertical,
			Horizontal
		}

		private bool ShowVerticalDirectionEditor => axis == Axis.Vertical;
        [ShowIf(nameof(ShowVerticalDirectionEditor)), LabelText("Direction")]
        public VerticalDirection verticalDirection;
		public enum VerticalDirection
		{
			Up,
			Down
		}

		private bool ShowHorizontalDirectionEditor => axis == Axis.Horizontal;
        [ShowIf(nameof(ShowHorizontalDirectionEditor)), LabelText("Direction")]
		public HorizontalDirection horizontalDirection;
        public enum HorizontalDirection
        {
            Right,
            Left
        }

        public float speed = 200;

		public AnimationCurve speedCurve;

		public Vector2Int referenceResolution = new Vector2Int(1280, 720);

		private RectTransform checkBounds;

		void Awake()
		{
            checkBounds = GetComponent<RectTransform>();
        }

		void Update()
		{
			#region Editor
#if UNITY_EDITOR

			isCapturingView			= false;
            positionFactorView		= -1;
            speedFactorView			= 0;
            directionFactorView		= 0;
#endif
            #endregion

            if (!display.IsDragging)
				return;

			var globalPosition = display.PointerPosition + display.TargetOffset;
			var isInBounds = RectTransformUtility.RectangleContainsScreenPoint(checkBounds, globalPosition);
			if (!isInBounds)
				return;

			RectTransformUtility.ScreenPointToLocalPointInRectangle(
				checkBounds, 
				globalPosition, 
				null, 
				out var localAbsolutePosition);

			var screenResolution = new Vector2(Screen.width, Screen.height);
			var scaleFactor = screenResolution / referenceResolution;
			var localNormalizedPosition = checkBounds.NormalizedRectPostion(localAbsolutePosition);

            var positionFactor = default(float);
			var directionFactor = default(int);
			var speedFactor = default(float);
			switch (axis)
			{
				case Axis.Vertical:
					switch (verticalDirection)
					{
						case VerticalDirection.Up:
							positionFactor = Mathf.InverseLerp(0, 1, localNormalizedPosition.y);
							directionFactor = -1;
                            break;

						case VerticalDirection.Down:
							positionFactor = Mathf.InverseLerp(1, 0, localNormalizedPosition.y);
							directionFactor = +1;
							break;
					}

					speedFactor = speedCurve.Evaluate(positionFactor) * speed * scaleFactor.y;
					var y = Mathf.Clamp(scroll.content.anchoredPosition.y + directionFactor * speedFactor * Time.deltaTime,
						0,
						scroll.viewport.rect.height); 
                    scroll.content.anchoredPosition = scroll.content.anchoredPosition.WithY(y);
                    break;

				case Axis.Horizontal:
					switch (horizontalDirection)
					{
						case HorizontalDirection.Right:
                            positionFactor = Mathf.InverseLerp(0, 1, localNormalizedPosition.x);
                            directionFactor = +1;
                            break;

						case HorizontalDirection.Left:
                            positionFactor = Mathf.InverseLerp(1, 0, localNormalizedPosition.x);
                            directionFactor = -1;
                            break;
					}

					speedFactor = speedCurve.Evaluate(positionFactor) * speed * scaleFactor.x;
					var x = Mathf.Clamp(scroll.content.anchoredPosition.x + directionFactor * speedFactor * Time.deltaTime,
						0,
                        scroll.viewport.rect.width);
                    scroll.content.anchoredPosition = scroll.content.anchoredPosition.WithX(x);
					break;
			}

            #region Editor
#if UNITY_EDITOR

            isCapturingView			= true;
            normalizedPositionView	= localNormalizedPosition;
            absolutePositionView	= localAbsolutePosition;
			positionFactorView		= positionFactor;
			speedFactorView			= speedFactor;
            directionFactorView		= directionFactor;
#endif
            #endregion
        }

		#region Editor
#if UNITY_EDITOR

		[FoldoutGroup("Debugging"), ShowInInspector, ReadOnly, LabelText("Is Capturing")]
		private bool isCapturingView;
		[FoldoutGroup("Debugging"), ShowInInspector, ReadOnly, LabelText("Absolute Local Pointer Position")]
        private Vector2 absolutePositionView;
		[FoldoutGroup("Debugging"), ShowInInspector, ReadOnly, LabelText("Normalized Local Pointer Position")]
        private Vector2 normalizedPositionView;
		[FoldoutGroup("Debugging"), ShowInInspector, ReadOnly, LabelText("Position Factor")]
        private float positionFactorView;
		[FoldoutGroup("Debugging"), ShowInInspector, ReadOnly, LabelText("Speed Factor")]
        private float speedFactorView;
		[FoldoutGroup("Debugging"), ShowInInspector, ReadOnly, LabelText("Direction Factor")]
		private int directionFactorView;

#endif
        #endregion
    }

	#region Editor
#if UNITY_EDITOR

	[CustomEditor(typeof(DragAndDropScrollHelper))]
	public class DragAndDropScrollHelperEditor : OdinEditor
	{
        private void OnSceneGUI()
        {
			Repaint();
        }
    }

#endif
    #endregion
}