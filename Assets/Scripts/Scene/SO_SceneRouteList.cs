using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneRouteList", menuName = "Scriptable Objects/NPC/Scene Route List")]
public class SO_SceneRouteList : ScriptableObject
{
    public List<SceneRoute> sceneRouteList;
}
