using NorskaLib.Extensions;
using NorskaLib.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace NorskaLib.UI.Markers
{
    public abstract class MarkersDisplay : MonoBehaviour
    {
        #region Dependencies

        /// <summary>
        /// Uses Camera.main by default. You may want to use custom dependency injection tool.
        /// </summary>
        protected virtual Camera ResolveCamera()
        {
            return Camera.main;
        }
        protected Camera Camera { get; private set; }
        protected Transform CameraTransform { get; private set; }

        #endregion

        [Tooltip("Determines normalized screen position, calculated via Camera.WorldToScreenPoint(MarkerEntry.PivotPosition). " +
            "Keep it slightly over 1.0, to avoid widgets dissapearing at the edge of the screen.")]
        [SerializeField] Vector2 offScreenMax = new (+1.05f, +1.05f);
        [Tooltip("Determines normalized screen position, calculated via Camera.WorldToScreenPoint(MarkerEntry.PivotPosition). " +
            "Keep it slightly below 0.0, to avoid widgets dissapearing at the edge of the screen.")]
        [SerializeField] Vector2 offScreenMin = new (-0.05f, -0.05f);

        [Tooltip("Determines offsets from screen edge (in RecTransform units), where compass-mode widgets are placed.")]
        [SerializeField] RectOffset compassPadding;

        protected abstract RectTransform DisplayTransform { get; }

        private Transform customCompassPivot;
        private Transform GetCompassPivot()
            => customCompassPivot == null ? CameraTransform : customCompassPivot;

        public void SetCompassPivot(Transform transform)
        {
            customCompassPivot = transform;
        }

        public void ResetCompassPivot()
        {
            customCompassPivot = null;
        }

        #region MonoBehaviour

        protected virtual void Start()
        {
            widgets   = new (100);
            sortDatas = new (100);

            Camera = ResolveCamera();
            CameraTransform = Camera.transform;

            MarkerEntry.onInstanceUnregistred += OnEntryUnregistred;
        }

        protected virtual void OnDestroy()
        {
            MarkerEntry.onInstanceUnregistred -= OnEntryUnregistred;
        }

        protected virtual void LateUpdate()
        {
            UpdateWidgets();
        }

        #endregion

        private Dictionary<MarkerEntry, MarkerWidget> widgets;
        private List<MarkerSortData> sortDatas;

        /// <summary>
        /// Override it to seprate entries among different Displays.
        /// </summary>
        protected virtual bool EntryIsValid(MarkerEntry entry) => true;

        private void OnEntryUnregistred(MarkerEntry entry)
        {
            if (!EntryIsValid(entry) || !widgets.TryGetValue(entry, out MarkerWidget widget))
                return;

            widgets.Remove(entry);

            if (widget != null)
            {
                widget.Unbind(entry);
                RemoveWidgetInstance(widget);
            }
        }

        protected abstract MarkerWidget GetWidgetInstance(MarkerEntry entry);

        protected abstract void RemoveWidgetInstance(MarkerWidget widget);

        private void UpdateWidgets()
        {
            var displayTransform = DisplayTransform;
            var displayRect = DisplayTransform.rect;
            var compassPivot = GetCompassPivot();

            foreach (var entry in MarkerEntry.Instances)
            {
                if (!EntryIsValid(entry))
                    continue;

                var mode = Camera.PointIsInsideViewport(entry.PivotPosition, offScreenMin, offScreenMax)
                    ? MarkerModes.World
                    : MarkerModes.Compass;
                var physicallyObscured = mode == MarkerModes.World
                    && entry is IPhysicallyObscuredMarkerEntry poEntry
                    && CheckPhysicalObscurance(poEntry, entry.PivotPosition);

                var show = entry.Show && EnumUtils.HasFlag((byte)entry.Modes, (byte)mode) && !physicallyObscured;

                var widgetExists = widgets.TryGetValue(entry, out var widget);
                if (!show && !widgetExists)
                {
                    continue;
                }
                else if (!show && widgetExists)
                {
                    widgets.Remove(entry);
                    widget.Unbind(entry);
                    RemoveWidgetInstance(widget);

                    continue;
                }
                else if (show && !widgetExists)
                {
                    widget = GetWidgetInstance(entry);
                    widget.Transform.SetParent(displayTransform);
                    widget.Transform.anchorMin = Vector2.zero;
                    widget.Transform.anchorMax = Vector2.zero;
                    widget.Bind(entry);
                    widgets.Add(entry, widget);
                }

                if (widget.lastMode != mode)
                    widget.SwitchMode(mode);

                var angleToEntry = MathUtils.AbsoluteSignedAngleXZ(compassPivot.position, entry.PivotPosition);
                if (widget is IAngleDisplayerWidget angleDisplayer)
                    angleDisplayer.Display(angleToEntry);

                var distance = Vector3.Distance(entry.PivotPosition, compassPivot.position);
                if (widget is IDistanceDisplayerWidget distanceDisplayer)
                    distanceDisplayer.Display(distance);

                if (widget is IFacingDisplayerWidget facingDisplayer)
                {
                    var facinfAngle = MathUtils.AbsoluteSignedAngleXZ(entry.transform);
                    facingDisplayer.Display(facinfAngle);
                }

                switch (mode)
                {
                    case MarkerModes.World:
                        var widgetScreenPos = Camera.WorldToScreenPointNormalized(entry.WidgetPosition);
                        widget.Transform.anchoredPosition = widgetScreenPos * displayRect.size;
                        break;

                    case MarkerModes.Compass:

                        var rectSize = displayRect.size - new Vector2(compassPadding.horizontal, compassPadding.vertical);
                        var rectCenter = new Vector2(compassPadding.left, compassPadding.top) + rectSize * 0.5f;
                        widget.Transform.anchoredPosition = MathUtils.PositionOnRectangle(rectCenter, rectSize, angleToEntry);
                        break;
                }

                sortDatas.Add(new MarkerSortData()
                {
                    widget      = widget,
                    distance    = distance
                });
            }

            sortDatas.Sort();
            for (int i = 0; i < sortDatas.Count; i++)
                sortDatas[i].widget.Transform.SetSiblingIndex(i);
            sortDatas.Clear();
        }

        private bool CheckPhysicalObscurance(IPhysicallyObscuredMarkerEntry entry, Vector3 entryPivotPosition)
        {
            if (entry.MultipleSamples)
            {
                foreach (var samplePoint in entry.SamplePoints)
                    if (!Physics.Linecast(CameraTransform.position, entryPivotPosition + samplePoint, entry.CollisionMask))
                        return false;

                return true;
            }
            else
            {
                return Physics.Linecast(CameraTransform.position, entryPivotPosition, entry.CollisionMask);
            }
        }
    }
}