using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDragItem
{
    public RectTransform DraggedRect { get; }

    public Action<IDragItem> OnBeginDrag { get; set; }
    public Action<IDragItem> OnEndDrag { get; set; }
}