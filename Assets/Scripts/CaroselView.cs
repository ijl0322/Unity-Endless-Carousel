using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class CaroselView : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    enum DragDirection { Left, Right };

    [SerializeField]
    private CaroselContent scrollContent;
    private ScrollRect scrollRect;
    private Vector2 lastDragPosition;
    private DragDirection dragDirection;
    public ITCaroselViewDataSource dataSource;

    private void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        scrollRect.vertical = false;
        scrollRect.horizontal = true;
        scrollRect.movementType = ScrollRect.MovementType.Unrestricted;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        lastDragPosition = eventData.position;
    }

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

    public void OnViewScroll()
    {
        HandleHorizontalScroll();
    }

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
        int currentItemCellIndex = swipeRight ? endItemContentIndex - 1 : endItemContentIndex + 1;
        CaroselCell currentCell = currItem.GetComponent<CaroselCell>();
        currentCell.cellIndex = currentItemCellIndex;
        currItem.SetSiblingIndex(endItemIndex);

        int totalNumberOfItems = dataSource.GetNumberOfItemsForCaroselView();
        int itemIndex = (currentItemCellIndex >= 0) ? currentItemCellIndex % totalNumberOfItems : totalNumberOfItems - Math.Abs(currentItemCellIndex % totalNumberOfItems) ;
        dataSource.UpdateCellInCaroselView(currentCell, itemIndex);
    }

    private bool ReachedThreshold(Transform item)
    {
        float posXThreshold = transform.position.x + scrollContent.Width * 0.5f;
        float negXThreshold = transform.position.x - scrollContent.Width * 0.5f;
        return dragDirection == DragDirection.Right ? item.position.x - scrollContent.ChildWidth * 0.5f > posXThreshold :
            item.position.x + scrollContent.ChildWidth * 0.5f < negXThreshold;
    }
}
