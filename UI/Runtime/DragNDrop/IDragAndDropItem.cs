using System;

namespace NorskaLib.UI.DragAndDrop
{
    public interface IDragAndDropItem 
    {
        /// <summary>
        /// Must wrap UnityEngine.EventSystems.IBeginDragHandler.OnBeginDrag callback.
        /// <para> NOTE: You need to implement all 3 interfaces IBeginDragHandler, IDragHandler and IEndDragHandler to make Unity invoke this callbacks. </para>
        /// </summary>
        public Action<IDragAndDropItem> OnStartsBeingDragged { get; set; }

        /// <summary>
        /// Must wrap UnityEngine.EventSystems.IEndDragHandler.OnEndDrag callback.
        /// <para> NOTE: You need to implement all 3 interfaces IBeginDragHandler, IDragHandler and IEndDragHandler to make Unity invoke this callbacks. </para>
        /// </summary>
        public Action<IDragAndDropItem> OnStopsBeingDragged { get; set; }
    }
}