using System;

namespace NorskaLib.UI.DragAndDrop
{
    public interface IDragAndDropItem 
    {
        /// <summary>
        /// Must wrap UnityEngine.EventSystems.IBeginDragHandler.OnBeginDrag callback.
        /// </summary>
        public Action<IDragAndDropItem> OnStartsBeingDragged { get; set; }

        /// <summary>
        /// Must wrap UnityEngine.EventSystems.IBeginDragHandler.OnEndDrag callback.
        /// </summary>
        public Action<IDragAndDropItem> OnStopsBeingDragged { get; set; }
    }
}