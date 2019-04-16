using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookExplorerController : MonoBehaviour, ITCaroselViewDataSource
{
    [SerializeField]
    private CaroselView bookCarosel;

    public int GetNumberOfRowsForCaroselView()
    {
        return 50;
    }

    public void UpdateCellInCaroselView(CaroselCell cell, int row)
    {
        Debug.LogFormat("Update cell {0}", row);
    }

    void Start()
    {
        //bookCarosel.dataSource = this;   
    }

    void Update()
    {
        
    }
}
