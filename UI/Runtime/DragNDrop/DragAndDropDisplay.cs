using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NorskaLib.UI.DragAndDrop
{
    // TO DO:
    // Add multiple canvas RenderMode support
    public abstract class DragAndDropDisplay : MonoBehaviour
    {
        protected abstract RectTransform DisplayTransform { get; }

        /// <summary>
        /// Must implement logic of resolving current mouse and/or touch position.
        /// </summary>
        public abstract Vector2 PointerPosition { get; }

        /// <summary>
        /// Offset from actual pointer position, added when calculating dragged item position;
        /// is useful for mobile devices, to prevent finger-covering.
        /// </summary>
        public virtual Vector2 ItemOffset => Vector2.zero;

        /// <summary>
        /// Offset from actual pointer position, added when calculating target;
        /// is useful for mobile devices, to prevent finger-covering.
        /// </summary>
        public virtual Vector2 TargetOffset => Vector2.zero;

        /// <summary>
        /// Uses Camera.main by default. You may want to use custom dependency injection tool.
        /// </summary>
        //protected virtual Camera ResolveCamera() 
        //{ 
        //    return Camera.main; 
        //}

        //public RenderMode canvasRenderMode;

        [ShowInInspector, ReadOnly]
        public bool IsDragging { get; private set; }

        [ShowInInspector, ReadOnly]
        public RectTransform DraggedTransform { get; private set; }

        public IDragAndDropItem CurrentItem { get; private set; }
        [ShowInInspector, ReadOnly, LabelText("Current Item")]
        public Object CurrentItemView => CurrentItem as Object;

        public IDragAndDropTarget CurrentTarget { get; private set; }
        [ShowInInspector, ReadOnly, LabelText("Current Target")]
        public Object CurrentTargetView => CurrentTarget as Object;

        private List<IDragAndDropItem> items;
        private List<RaycastResult> raycastResults;

        protected virtual void Awake()
        {
            raycastResults = new List<RaycastResult>(16);
            items = new List<IDragAndDropItem>(16);
        }

        protected virtual void Update()
        {
            if (!IsDragging)
                return;

            var pointerPosition = PointerPosition;
            var itemScreenPosition = pointerPosition + ItemOffset;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                DisplayTransform,
                itemScreenPosition,
                null,
                out var itemAnchoredPosition);
            DraggedTransform.anchoredPosition = itemAnchoredPosition;

            // TO DO:
            // Move to Utils static function
            var targetPointerData = new PointerEventData(EventSystem.current)
            {
                position = PointerPosition + TargetOffset
            };
            raycastResults.Clear();
            EventSystem.current.RaycastAll(targetPointerData, raycastResults);
            var detectedTarget = default(IDragAndDropTarget);
            var anyTarget = raycastResults.Count > 0 && raycastResults[0].gameObject.TryGetComponent(out detectedTarget);
            if (anyTarget && detectedTarget != CurrentTarget)
            {
                if (CurrentTarget == null)
                {
                    detectedTarget.OnStartsBeingTargeted();
                    OnTargetAquired(pointerPosition, detectedTarget);
                }
                else
                {
                    CurrentTarget.OnStopsBeingTargeted();
                    detectedTarget.OnStartsBeingTargeted();
                    OnTargetChanged(pointerPosition, CurrentTarget, detectedTarget);
                }

                CurrentTarget = detectedTarget;
            }
            else if (!anyTarget && CurrentTarget != null)
            {
                OnTargetLost(pointerPosition, CurrentTarget);
                CurrentTarget.OnStopsBeingTargeted();
                CurrentTarget = null;
            }
        }

        protected virtual void OnDestroy()
        {
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                item.OnStartsBeingDragged -= OnItemDragBegins;
                item.OnStopsBeingDragged -= OnItemDragEnds;
            }
            items.Clear();
        }

        void OnItemDragBegins(IDragAndDropItem draggedItem)
        {
            IsDragging = true;
            CurrentItem = draggedItem;

            DraggedTransform = ResolveDraggedTransform(draggedItem);
            OnDragStarted(PointerPosition, draggedItem);
        }
        public abstract RectTransform ResolveDraggedTransform(IDragAndDropItem draggedItem);

        void OnItemDragEnds(IDragAndDropItem draggedItem)
        {
            var pointerPosition = PointerPosition + TargetOffset;
            if (CurrentTarget != null)
            {
                CurrentTarget.OnStopsBeingTargeted();
                OnDragFinished(pointerPosition, draggedItem, CurrentTarget);
            }
            else
                OnDragCanceled(pointerPosition, draggedItem);

            IsDragging = false;
            CurrentItem = null;
            CurrentTarget = null;

            DisposeDraggedTransform(draggedItem);
            DraggedTransform = null;
        }
        public abstract void DisposeDraggedTransform(IDragAndDropItem draggedItem);

        public void RegisterItem(IDragAndDropItem item) 
        {
            if (items.Contains(item))
                return;

            items.Add(item);
            item.OnStartsBeingDragged += OnItemDragBegins;
            item.OnStopsBeingDragged += OnItemDragEnds;
        }

        public void UnregisterItem(IDragAndDropItem item)
        {
            var check = items.Remove(item);
            if (!check)
                return;

            item.OnStartsBeingDragged -= OnItemDragBegins;
            item.OnStopsBeingDragged -= OnItemDragEnds;
        }

        protected virtual void OnDragStarted(Vector2 pointerPosition, IDragAndDropItem item) { }
        protected virtual void OnDragCanceled(Vector2 pointerPosition, IDragAndDropItem item) { }
        protected virtual void OnDragFinished(Vector2 pointerPosition, IDragAndDropItem item, IDragAndDropTarget target) { }

        protected virtual void OnTargetAquired(Vector2 pointerPosition, IDragAndDropTarget target) { }
        protected virtual void OnTargetLost(Vector2 pointerPosition, IDragAndDropTarget target) { }
        protected virtual void OnTargetChanged(Vector2 pointerPosition, IDragAndDropTarget oldTarget, IDragAndDropTarget newTarget) { }
    } 
}