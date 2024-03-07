[System.Serializable]
public class GridPropertyDetails
{
    public int gridX;
    public int gridY;
    public bool isDiggable = false;
    public bool canDropItem = false;
    public bool canHatchEggs = false; //ø…“‘∑ıªØº¶µ∞
    public bool canPlaceFurntiure = false;
    public bool isPath = false;
    public bool isNPCObstacle = false;
    public int daysSinceDug = -1;
    public int daysSinceWatered = -1;
    public int daysSinceHatched = -1;
    public int eggItemCode = -1;
    public int seedItemCode = -1;
    public int growthDays = -1;
    public int daysSinceLastHarvest = -1;

    public GridPropertyDetails()
    {

    }
}
