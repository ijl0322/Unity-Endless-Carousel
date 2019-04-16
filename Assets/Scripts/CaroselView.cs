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
        CellContentInitialSetup();
    }

    private void CellContentInitialSetup()
    {
        scrollContent.CellsInitialSetup();
        CaroselCell[] caroselCells = scrollContent.CaroselCells;
        for (int i = 0; i < caroselCells.Length; i++)
        {
            dataSource.UpdateCellInCaroselView(caroselCells[i], i);
        }
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

    // Call this function in Scroll View > On Value Change
    public void OnViewScroll()
    {
        HandleHorizontalScroll();
    }

    private void HandleHorizontalScroll()
    {   
        bool swipeRight = dragDirection == DragDirection.Right;

        // Getting the rightmost or leftmost cell Object depending on swipe direction
        int currentPrefabIndex = swipeRight ? scrollRect.content.childCount - 1 : 0;
        var currentItemTransform = scrollRect.content.GetChild(currentPrefabIndex);

        // Determine whether the cell needs repositioning
        if (!ReachedThreshold(currentItemTransform)) { return; }

        // Find the cell object which the current object should be positioned next to
        int endPrefabIndex = swipeRight ? 0 : scrollRect.content.childCount - 1;
        Transform endItemTransform = scrollRect.content.GetChild(endPrefabIndex);

        // Calculate the new position for the cell
        Vector2 newPos = endItemTransform.position;
        newPos.x = swipeRight 
            ? endItemTransform.position.x - (scrollContent.ChildWidth + scrollContent.ItemSpacing)
            : endItemTransform.position.x + (scrollContent.ChildWidth + scrollContent.ItemSpacing);
        currentItemTransform.position = newPos;

        // Assign new cell index to the current cell
        int endItemCellIndex = endItemTransform.GetComponent<CaroselCell>().cellIndex;
        int currentItemCellIndex = swipeRight ? endItemCellIndex - 1 : endItemCellIndex + 1;
        CaroselCell currentCell = currentItemTransform.GetComponent<CaroselCell>();
        currentCell.cellIndex = currentItemCellIndex;
        currentItemTransform.SetSiblingIndex(endPrefabIndex);


        // Update cell content according to the new index
        // Since it's an endless scroll, the currentItemCellIndex can be very large or very small
        // Take modular to decide what content to display
        int totalNumberOfItems = dataSource.GetNumberOfItemsForCaroselView();
        int contentIndex = (currentItemCellIndex >= 0) ? currentItemCellIndex % totalNumberOfItems : totalNumberOfItems - Math.Abs(currentItemCellIndex % totalNumberOfItems) ;
        dataSource.UpdateCellInCaroselView(currentCell, contentIndex);
    }

    // Returns true if item needs to be repositioned
    private bool ReachedThreshold(Transform item)
    {
        float posXThreshold = transform.position.x + scrollContent.Width * 0.5f;
        float negXThreshold = transform.position.x - scrollContent.Width * 0.5f;
        return dragDirection == DragDirection.Right 
            ? item.position.x - scrollContent.ChildWidth * 0.5f > posXThreshold 
            :item.position.x + scrollContent.ChildWidth * 0.5f < negXThreshold;
    }
}
