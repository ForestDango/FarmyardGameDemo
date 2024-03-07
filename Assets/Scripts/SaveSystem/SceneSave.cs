using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SceneSave 
{
    public Dictionary<string, bool> boolDictionary;
    public List<SceneItem> listSceneItem;
    public Dictionary<string, GridPropertyDetails> gridPropetyDeatilsDictionary;
    public Dictionary<string, string> stringDictionary;
    public Dictionary<string, int> intDictionary;
    public Dictionary<string, int[]> intArrayDicitionary;
    public Dictionary<string, Vector3Serializable> vector3Dictionary;
    public List<InventoryItem>[] listInventoryItemArray;

}
