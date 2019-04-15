using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CaroselView : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    enum DragDirection { Left, Right };
    #region Private Members

    /// <summary>
    /// The ScrollContent component that belongs to the scroll content GameObject.
    /// </summary>
    [SerializeField]
    private CaroselContent scrollContent;

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
        bool swipeRight = dragDirection == DragDirection.Right;
        int currItemIndex = swipeRight ? scrollRect.content.childCount - 1 : 0;
        var currItem = scrollRect.content.GetChild(currItemIndex);
        if (!ReachedThreshold(currItem))
        {
            return;
        }

        int endItemIndex = swipeRight ? 0 : scrollRect.content.childCount - 1;
        Transform endItem = scrollRect.content.GetChild(endItemIndex);
        Vector2 newPos = endItem.position;

        if (swipeRight)
        {
            newPos.x = endItem.position.x - (scrollContent.ChildWidth + scrollContent.ItemSpacing);
        }
        else
        {
            newPos.x = endItem.position.x + (scrollContent.ChildWidth + scrollContent.ItemSpacing);
        }
        currItem.position = newPos;
        int endItemContentIndex = endItem.GetComponent<CaroselCell>().cellIndex;
        currItem.GetComponent<CaroselCell>().cellIndex = swipeRight ? endItemContentIndex - 1 : endItemContentIndex + 1;
        currItem.GetComponent<CaroselCell>().bookTitleText.text = "Book " + (swipeRight ? endItemContentIndex - 1 : endItemContentIndex + 1);
        currItem.SetSiblingIndex(endItemIndex);
    }

    private bool ReachedThreshold(Transform item)
    {
        float posXThreshold = transform.position.x + scrollContent.Width * 0.5f;
        float negXThreshold = transform.position.x - scrollContent.Width * 0.5f;
        return dragDirection == DragDirection.Right ? item.position.x - scrollContent.ChildWidth * 0.5f > posXThreshold :
            item.position.x + scrollContent.ChildWidth * 0.5f < negXThreshold;
    }
}
