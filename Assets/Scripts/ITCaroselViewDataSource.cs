public interface ITCaroselViewDataSource
{
    int GetNumberOfItemsForCaroselView();
    void UpdateCellInCaroselView(CaroselCell cell, int row);
}