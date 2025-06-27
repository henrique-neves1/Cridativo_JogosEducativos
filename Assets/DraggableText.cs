using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableText : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string numberValue; // e.g., "sete"
    public Transform originalParent;
    public Vector3 originalPosition;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    [HideInInspector]
    public bool wasDroppedOnSlot = false;
    public int originalSiblingIndex;

    private void Awake()
    {
        originalParent = transform.parent;
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        wasDroppedOnSlot = false;
        originalParent = transform.parent;
        originalPosition = transform.position;
        originalSiblingIndex = transform.GetSiblingIndex(); // Store index
        transform.SetParent(canvas.transform); // bring to top
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        if (!wasDroppedOnSlot)
        {
            // Not dropped on a valid slot → return to layout
            transform.SetParent(originalParent, false); // Reinsert into grid
            //transform.SetAsLastSibling(); // Optional: put it at the end
            transform.SetSiblingIndex(originalSiblingIndex); // Restore position
            transform.localScale = Vector3.one;
        }
    }
}
