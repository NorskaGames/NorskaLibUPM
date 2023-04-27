using NorskaLib.Extensions;
using NorskaLib.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// TO DO:
// Fix sorting
namespace NorskaLib.UI
{
    using SortData  = MarkerWidgetSortData;
    using Widget    = MarkerWidget;
    using Entry     = MarkerEntry;

    public abstract class MarkersOverlay : Window
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
        [SerializeField] Vector2 offScreenMax = Vector2.one;
        [Tooltip("Determines normalized screen position, calculated via Camera.WorldToScreenPoint(MarkerEntry.PivotPosition). " +
            "Keep it slightly below 0.0, to avoid widgets dissapearing at the edge of the screen.")]
        [SerializeField] Vector2 offScreenMin = Vector2.zero;

        [Tooltip("Determines offsets from screen edge (in RecTransform units), where compass-widgets are placed.")]
        [SerializeField] RectOffset compassPadding;

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
            widgets             = new (100);
            sortDatas           = new (100);

            Camera = ResolveCamera();
            CameraTransform = Camera.transform;

            Entry.onInstanceUnregistred += OnEntryUnregistred;
        }

        protected virtual void OnDestroy()
        {
            Entry.onInstanceUnregistred -= OnEntryUnregistred;
        }

        protected virtual void LateUpdate()
        {
            UpdateWidgets();
        }

        #endregion

        private Dictionary<Entry, Widget> widgets;
        private List<SortData> sortDatas;

        private void OnEntryUnregistred(Entry entry)
        {
            if (!widgets.TryGetValue(entry, out Widget widget) || widget == null)
                return;

            widgets.Remove(entry);
            widget.Unbind();
            RemoveWidgetInstance(widget);
        }

        protected abstract Widget GetWidgetInstance(Entry entry);

        protected abstract void RemoveWidgetInstance(Widget widget);

        private void UpdateWidgets()
        {
            var screenRect = transform.rect;
            var compassPivot = GetCompassPivot();

            foreach (var entry in Entry.Instances)
            {
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
                    widget.Unbind();
                    RemoveWidgetInstance(widget);

                    continue;
                }
                else if (show && !widgetExists)
                {
                    widget = GetWidgetInstance(entry);
                    widget.Transform.anchorMin = Vector2.zero;
                    widget.Transform.anchorMax = Vector2.zero;
                    widget.Bind(entry);
                    widgets.Add(entry, widget);
                }

                if (widget.lastMode != mode)
                    widget.SwitchMode(mode);

                switch (mode)
                {
                    case MarkerModes.World:
                        var widgetScreenPos = Camera.WorldToScreenPointNormalized(entry.WidgetPosition);
                        widget.Transform.anchoredPosition = widgetScreenPos * screenRect.size;
                        break;

                    case MarkerModes.Compass:
                        var angle = MathUtils.AbsoluteSignedAngleXZ(compassPivot.position, entry.PivotPosition);
                        if (widget is IAngleDisplayerWidget angleDisplayer)
                            angleDisplayer.DisplayAngle(angle);

                        var rectSize = screenRect.size - new Vector2(compassPadding.horizontal, compassPadding.vertical);
                        var rectCenter = new Vector2(compassPadding.left, compassPadding.top) + rectSize * 0.5f;
                        widget.Transform.anchoredPosition = MathUtils.PositionOnRectangle(rectCenter, rectSize, angle);
                        break;
                }

                var distance = Vector3.Distance(entry.PivotPosition, compassPivot.position);
                if (widget is IDistanceDisplayerWidget distanceDisplayer)
                    distanceDisplayer.DisplayDistance(distance);

                sortDatas.Add(new SortData()
                {
                    widget = widget,
                    distance = distance
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