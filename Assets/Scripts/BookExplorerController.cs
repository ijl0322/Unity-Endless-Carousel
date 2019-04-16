using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookExplorerController : MonoBehaviour, ITCaroselViewDataSource
{
    [SerializeField]
    private CaroselView bookCarosel;

    void Start()
    {
        bookCarosel.dataSource = this;   
    }

    void Update()
    {
        
    }

    public int GetNumberOfItemsForCaroselView()
    {
        return 50;
    }

    public void UpdateCellInCaroselView(CaroselCell cell, int row)
    {
        BookCell bookCell = cell as BookCell;
        bookCell.bookTitle.text = "Book " + row;
    }
}
