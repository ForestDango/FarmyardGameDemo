[System.Serializable]
public class GridPropetry 
{
    public GridCoordinate gridCoordinate;
    public GridBoolProperty gridBoolProperty;
    public bool gridBoolValue = false;

    public GridPropetry(GridCoordinate gridCoordinate,GridBoolProperty gridBoolProperty,bool gridBoolValue)
    {
        this.gridCoordinate = gridCoordinate;
        this.gridBoolProperty = gridBoolProperty;
        this.gridBoolValue = gridBoolValue;
    }
}
