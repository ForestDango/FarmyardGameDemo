using System.Collections.Generic;

[System.Serializable]
public class SceneRoute 
{
    public SceneName fromSceneName;
    public SceneName toSceneName;
    public List<ScenePath> scenePathList;
}
