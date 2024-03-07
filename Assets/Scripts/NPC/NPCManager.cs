using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
[RequireComponent(typeof(AStar))]
public class NPCManager : Singleton<NPCManager>
{
    [SerializeField] private SO_SceneRouteList so_SceneRouteList;
    private Dictionary<string, SceneRoute> sceneRouteDictionary;

    [HideInInspector]public NPC[] npcArray;

    private AStar aStar;

    protected override void Awake()
    {
        base.Awake();

        sceneRouteDictionary = new Dictionary<string, SceneRoute>();

        if (so_SceneRouteList.sceneRouteList.Count > 0)
        {
            foreach (SceneRoute so_SceneRoute in so_SceneRouteList.sceneRouteList)
            {
                if (sceneRouteDictionary.ContainsKey(so_SceneRoute.fromSceneName.ToString() + so_SceneRoute.toSceneName.ToString()))
                {
                    Debug.Log("Dupilcate Scene Route Key Found Please Check");
                    continue;
                }
                sceneRouteDictionary.Add(so_SceneRoute.fromSceneName.ToString() + so_SceneRoute.toSceneName.ToString(), so_SceneRoute);
            }
        }

        npcArray = FindObjectsOfType<NPC>();
        aStar = GetComponent<AStar>();
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += AfterSceneLoaded;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoaded;
    }

    private void AfterSceneLoaded()
    {
        SetNpcActiveStatus();
    }

    private void SetNpcActiveStatus()
    {
        foreach (NPC npc in npcArray)
        {
            NPCMovement npcMovement = npc.GetComponent<NPCMovement>();

            if (npcMovement.npcCurrentScene.ToString() == SceneManager.GetActiveScene().name)
            {
                npcMovement.SetNPCActiveInScene();
            }
            else
            {
                npcMovement.SetNPCInActiveInScene();
            }
        }
    }

    public bool BuilPath(SceneName sceneName,Vector2Int startGridPosition,Vector2Int endGridPosition,Stack<NPCMovementStep> npcMovementStepStack)
    {
        if (aStar.BuildPath(sceneName, startGridPosition, endGridPosition, npcMovementStepStack))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public SceneRoute GetSceneRoute(string fromSceneName,string toSceneName)
    {
        SceneRoute sceneRoute;
        if(sceneRouteDictionary.TryGetValue(fromSceneName +  toSceneName,out sceneRoute))
        {
            return sceneRoute;
        }
        else
        {
            return null;
        }
    }
}
