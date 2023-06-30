namespace NorskaLib.UI.DragAndDrop
{
    public interface IDragAndDropTarget
	{
        /// <summary>
        /// Is called by DragAndDropDisplay when user drags an item over this UI element.
        /// </summary>
        void OnStartsBeingTargeted();

		/// <summary>
		/// Is called by DragAndDropDisplay when pointer leaves this UI element or user stops dragging and item.
		/// </summary>
		void OnStopsBeingTargeted();
    } 
}