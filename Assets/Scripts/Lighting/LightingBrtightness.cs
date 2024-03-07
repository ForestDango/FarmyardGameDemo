/// <summary>
/// 数据结集合结构体，记录Light的季节，小时，强度
/// </summary>
[System.Serializable]
public struct LightingBrtightness
{
    public Season season;
    public int hour;
    public float lightIndensity;
}
