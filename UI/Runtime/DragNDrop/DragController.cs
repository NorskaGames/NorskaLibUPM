using NorskaLib.Utilities;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class DragController : MonoBehaviour
{
    #region Dependencies

    #endregion

    [SerializeField] RectTransform itemParentRect;

    public bool IsDragging { get; private set; }
    public IDragItem DraggedItem { get; private set; }
    public IDragTarget DragTarget { get; private set; }
    public RectTransform ItemClone { get; private set; }

    public event Action<IDragItem, Vector2>                  onBeginDrag;
    public event Action<IDragItem, Vector2>                  onInterruptDrag;
    public event Action<IDragItem, IDragTarget, Vector2>     onFinishDrag;
    /// <summary>
    /// Params: old target, new target
    /// </summary>
    public event Action<IDragTarget, IDragTarget, Vector2>   onTargetChanged;
    public event Action<IDragTarget, Vector2>                onTargetAquired;
    public event Action<IDragTarget, Vector2>                onTargetLost;

    private List<IDragItem> items;
    private RenderMode renderMode;
    private Camera cameraUI;

    public void Initialize(RenderMode renderMode, Camera cameraUI)
    {
        this.renderMode = renderMode;
        this.cameraUI = cameraUI;
    }

    public void Setup(IEnumerable<IDragItem> items)
    {
        foreach (var item in items)
        {
            item.OnBeginDrag += OnBeginDragWidget;
            item.OnEndDrag   += OnEndDragWidget;
        }
        this.items.AddRange(items);
    }

    public void Clear()
    {
        foreach (var obj in items)
        {
            if (obj as MonoBehaviour == null)
                continue;

            obj.OnBeginDrag -= OnBeginDragWidget;
            obj.OnEndDrag   -= OnEndDragWidget;
        }

        items.Clear();
    }

    #region MonoBehaviour

    void Awake()
    {
        items = new List<IDragItem>();
    }

    void Update()
    {
        if (!IsDragging || Touch.activeFingers.Count <= 0)
            return;

        Vector2 pointerPosition = Touch.activeFingers[0].screenPosition;

        #region Updating position

        Vector2 anchoredPosition;

        switch (renderMode)
        {
            case RenderMode.ScreenSpaceOverlay:
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    itemParentRect,
                    pointerPosition,
                    null,
                    out anchoredPosition);
                break;

            default:
            case RenderMode.ScreenSpaceCamera:
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    itemParentRect,
                    pointerPosition,
                    cameraUI,
                    out anchoredPosition);
                break;
        }

        ItemClone.anchoredPosition = anchoredPosition;

        #endregion

        #region Updating target

        var pointerTargetObj = NorskaLib.UI.UIUtils.GetObject(pointerPosition);
        if (pointerTargetObj == null || !pointerTargetObj.TryGetComponent<IDragTarget>(out var newTarget))
        {
            if (DragTarget == null)
                return;
            
            onTargetLost?.Invoke(DragTarget, pointerPosition);
            DragTarget = null;
        }
        else
        {
            if (DragTarget == newTarget)
                return;

            if (DragTarget == null && newTarget != null)
                onTargetAquired?.Invoke(newTarget, pointerPosition);
            else if (DragTarget != null && newTarget == null)
                onTargetLost?.Invoke(DragTarget, pointerPosition);
            else
                onTargetChanged?.Invoke(DragTarget, newTarget, pointerPosition);

            DragTarget = newTarget;
        }

        #endregion

    }

    #endregion

    #region Callbacks

    void OnBeginDragWidget(IDragItem item)
    {
        IsDragging = true;
        DraggedItem = item;

        ItemClone = Instantiate(item.DraggedRect, item.DraggedRect.parent);

        ItemClone.pivot = new Vector2(0.5f, 0.5f);
        ItemClone.anchorMin = new Vector2(0.5f, 0.5f);
        ItemClone.anchorMax = new Vector2(0.5f, 0.5f);
        ItemClone.sizeDelta = item.DraggedRect.rect.size;

        ItemClone.SetParent(itemParentRect);
        ItemClone.localPosition = new Vector3(
            ItemClone.localPosition.x,
            ItemClone.localPosition.y,
            0);

        var pointerPos = Touch.activeFingers.Count <= 0
            ? Vector2.zero
            : Touch.activeFingers[0].screenPosition;
        onBeginDrag?.Invoke(item, pointerPos);
    }

    void OnEndDragWidget(IDragItem item)
    {
        var pointerPos = Touch.activeFingers.Count <= 0
            ? Vector2.zero
            : Touch.activeFingers[0].screenPosition;

        if (DragTarget != null)
            onFinishDrag?.Invoke(item, DragTarget, pointerPos);
        else
            onInterruptDrag?.Invoke(item, pointerPos);

        IsDragging = false;
        DraggedItem = null;
        DragTarget = null;

        Destroy(ItemClone.gameObject);
    }

    #endregion

    #region Editor only
#if UNITY_EDITOR

    [Header("Debugging")]

    [ShowInInspector][DisableIf("@true")]
    private bool isDraggingView;
    [ShowInInspector][DisableIf("@true")]
    private MonoBehaviour draggedItemView;
    [ShowInInspector][DisableIf("@true")]
    private MonoBehaviour dragTargetView;

    void LateUpdate()
    {
        isDraggingView = IsDragging;
        draggedItemView = DraggedItem as MonoBehaviour;
        dragTargetView = DragTarget as MonoBehaviour;
    }

#endif
    #endregion
}