using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CaroselView : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    enum DragDirection { Left, Right, None};
    #region Private Members

    /// <summary>
    /// The ScrollContent component that belongs to the scroll content GameObject.
    /// </summary>
    [SerializeField]
    private CaroselContent scrollContent;

    /// <summary>
    /// How far the items will travel outside of the scroll view before being repositioned.
    /// </summary>
    [SerializeField]
    private float outOfBoundsThreshold;

    /// <summary>
    /// The ScrollRect component for this GameObject.
    /// </summary>
    private ScrollRect scrollRect;

    /// <summary>
    /// The last position where the user has dragged.
    /// </summary>
    private Vector2 lastDragPosition;

    /// <summary>
    /// Is the user dragging in the positive axis or the negative axis?
    /// </summary>
    private DragDirection dragDirection;

    #endregion

    private void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        scrollRect.vertical = false;
        scrollRect.horizontal = true;
        scrollRect.movementType = ScrollRect.MovementType.Unrestricted;
    }

    /// <summary>
    /// Called when the user starts to drag the scroll view.
    /// </summary>
    /// <param name="eventData">The data related to the drag event.</param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        lastDragPosition = eventData.position;
    }

    /// <summary>
    /// Called while the user is dragging the scroll view.
    /// </summary>
    /// <param name="eventData">The data related to the drag event.</param>
    public void OnDrag(PointerEventData eventData)
    { 
        if (eventData.position.x > lastDragPosition.x)
        {
            dragDirection = DragDirection.Right;
            lastDragPosition = eventData.position;
        } 
        else if (eventData.position.x < lastDragPosition.x) 
        {
            dragDirection = DragDirection.Left;
            lastDragPosition = eventData.position;
        } 
    }

    /// <summary>
    /// Called when the user is dragging/scrolling the scroll view.
    /// </summary>
    public void OnViewScroll()
    {
        HandleHorizontalScroll();
    }

    /// <summary>
    /// Called if the scroll view is oriented horizontally.
    /// </summary>
    private void HandleHorizontalScroll()
    {
        int currItemIndex = dragDirection == DragDirection.Right ? scrollRect.content.childCount - 1 : 0;
        var currItem = scrollRect.content.GetChild(currItemIndex);
        if (!ReachedThreshold(currItem))
        {
            return;
        }

        int endItemIndex = dragDirection == DragDirection.Right ? 0 : scrollRect.content.childCount - 1;
        Transform endItem = scrollRect.content.GetChild(endItemIndex);
        Vector2 newPos = endItem.position;

        if (dragDirection == DragDirection.Right)
        {
            //newPos.x = endItem.position.x - scrollContent.ChildWidth * 1.5f + scrollContent.ItemSpacing;
            newPos.x = endItem.position.x - (scrollContent.ChildWidth * 1f + scrollContent.ItemSpacing);
        }
        else
        {
            newPos.x = endItem.position.x + (scrollContent.ChildWidth * 1f + scrollContent.ItemSpacing);
        }

        Debug.LogFormat("end item pos {0}, new item pos {1}", endItem.position.x, newPos.x);
        currItem.position = newPos;
        currItem.SetSiblingIndex(endItemIndex);
    }

    /// <summary>
    /// Checks if an item has the reached the out of bounds threshold for the scroll view.
    /// </summary>
    /// <param name="item">The item to be checked.</param>
    /// <returns>True if the item has reached the threshold for either ends of the scroll view, false otherwise.</returns>
    private bool ReachedThreshold(Transform item)
    {
        float posXThreshold = transform.position.x + scrollContent.Width * 0.5f + outOfBoundsThreshold;
        float negXThreshold = transform.position.x - scrollContent.Width * 0.5f - outOfBoundsThreshold;
        return dragDirection == DragDirection.Right ? item.position.x - scrollContent.ChildWidth * 0.5f > posXThreshold :
            item.position.x + scrollContent.ChildWidth * 0.5f < negXThreshold;
    }
}
