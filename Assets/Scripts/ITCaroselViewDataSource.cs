public interface ITCaroselViewDataSource
{
    int GetNumberOfRowsForCaroselView();
    void UpdateCellInCaroselView(CaroselCell cell, int row);
}