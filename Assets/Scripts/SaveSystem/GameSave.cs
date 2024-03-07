using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameSave
{
    public Dictionary<string, GameObjectSave> gameObjectData;

    public GameSave()
    {
        gameObjectData = new Dictionary<string, GameObjectSave>();
    }
}
