using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CaroselContent : MonoBehaviour
{
    #region Public Properties

    public float ItemSpacing { get { return itemSpacing; } }

    /// <summary>
    /// The width of the scroll content.
    /// </summary>
    public float Width { get { return width; } }

    /// <summary>
    /// The height of the scroll content.
    /// </summary>
    public float Height { get { return height; } }

    /// <summary>
    /// The width for each child of the scroll view.
    /// </summary>
    public float ChildWidth { get { return childWidth; } }

    /// <summary>
    /// The height for each child of the scroll view.
    /// </summary>
    public float ChildHeight { get { return childHeight; } }

    public int numberOfCellPrefabs;

    #endregion

    #region Private Members

    /// <summary>
    /// The RectTransform component of the scroll content.
    /// </summary>
    private RectTransform rectTransform;

    /// <summary>
    /// The RectTransform components of all the children of this GameObject.
    /// </summary>
    private CaroselCell[] caroselCells;

    /// <summary>
    /// The width and height of the parent.
    /// </summary>
    private float width, height;

    /// <summary>
    /// The width and height of the children GameObjects.
    /// </summary>
    private float childWidth, childHeight;

    /// <summary>
    /// How far apart each item is in the scroll view.
    /// </summary>
    [SerializeField]
    private float itemSpacing;

    /// <summary>
    /// How much the items are indented from the top/bottom and left/right of the scroll view.
    /// </summary>
    [SerializeField]
    private float horizontalMargin, verticalMargin;

    [SerializeField]
    private GameObject cellPrefab;

    #endregion

    private void Start()
    {
        if (numberOfCellPrefabs == 0) { return; }
        rectTransform = GetComponent<RectTransform>();
        caroselCells = new CaroselCell[numberOfCellPrefabs];

        for (int i = 0; i < numberOfCellPrefabs; i++)
        {
            CaroselCell cell = Instantiate(cellPrefab).GetComponent<CaroselCell>();
            cell.name = "Cell_" + i;
            cell.transform.SetParent(rectTransform, false);
            caroselCells[i] = cell;
        }

        width = rectTransform.rect.width;
        height = rectTransform.rect.height;

        childWidth = caroselCells[0].GetComponent<RectTransform>().rect.width;
        childHeight = caroselCells[0].GetComponent<RectTransform>().rect.height;

        float posOffset = childWidth * 0.5f;
        for (int i = 0; i < caroselCells.Length; i++)
        {
            caroselCells[i].cellIndex = i;
            Vector2 childPos = caroselCells[i].GetComponent<RectTransform>().localPosition;
            childPos.x = posOffset + i * (childWidth + itemSpacing);
            caroselCells[i].GetComponent<RectTransform>().localPosition = childPos;
        }
    }
}
