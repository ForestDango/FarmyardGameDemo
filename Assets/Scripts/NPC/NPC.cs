using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(NPCMovement))]
[RequireComponent(typeof(GenerateGUID))]
public class NPC : MonoBehaviour,ISaveable
{
    private string _iSaveableUniqueID;

    public string ISaveableUniqueID { get => _iSaveableUniqueID; set => _iSaveableUniqueID = value; }

    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get => _gameObjectSave; set => _gameObjectSave = value; }

    private NPCMovement npcMovement;

    private void OnEnable()
    {
        ISaveableRegister();
    }

    private void OnDisable()
    {
        ISaveableDeregister();
    }

    private void Awake()
    {
        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void Start()
    {
        npcMovement = GetComponent<NPCMovement>();
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if (gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            if (gameObjectSave.sceneData.TryGetValue(Settings.persistentScene, out SceneSave sceneSave))
            {
                if (sceneSave.vector3Dictionary != null && sceneSave.stringDictionary != null)
                {
                    if(sceneSave.vector3Dictionary .TryGetValue("npcTargetGridPositon",out Vector3Serializable savedNPCTargetGridPosition))
                    {
                        npcMovement.npcTargetGridPosition = new Vector3Int((int)savedNPCTargetGridPosition.x, (int)savedNPCTargetGridPosition.y, (int)savedNPCTargetGridPosition.z);
                        npcMovement.npcCurrentGridPosition = npcMovement.npcTargetGridPosition;
                    }

                    if (sceneSave.vector3Dictionary.TryGetValue("npcTargetWorldPositon ", out Vector3Serializable savedNPCTargetWorldPosition))
                    {
                        npcMovement.npcTargetWorldPosition = new Vector3(savedNPCTargetWorldPosition.x, savedNPCTargetWorldPosition.y, savedNPCTargetWorldPosition.z);
                        transform.position = npcMovement.npcTargetWorldPosition;
                    }

                    if (sceneSave.stringDictionary.TryGetValue("npcTargetScene", out string savedTargetScene))
                    {
                        if (Enum.TryParse<SceneName>(savedTargetScene, out SceneName sceneName))
                        {
                            npcMovement.npcTargetScene = sceneName;
                            npcMovement.npcCurrentScene = npcMovement.npcTargetScene;
                        }
                    }

                    npcMovement.CancelNPCMovement();
                }                
            }
        }
    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        //Do Nothing
    }

    public GameObjectSave ISaveableSave()
    {
        GameObjectSave.sceneData.Remove(Settings.persistentScene);
        SceneSave sceneSave = new SceneSave();
        sceneSave.vector3Dictionary = new Dictionary<string, Vector3Serializable>();
        sceneSave.stringDictionary = new Dictionary<string, string>();

        sceneSave.vector3Dictionary.Add("npcTargetGridPositon", 
            new Vector3Serializable(npcMovement.npcTargetGridPosition.x, npcMovement.npcTargetGridPosition.y, npcMovement.npcTargetGridPosition.z));
        sceneSave.vector3Dictionary.Add("npcTargetWorldPositon",
            new Vector3Serializable(npcMovement.npcTargetWorldPosition.x, npcMovement.npcTargetWorldPosition.y, npcMovement.npcTargetWorldPosition.z));
        sceneSave.stringDictionary.Add("npcTargetScene", npcMovement.npcTargetScene.ToString());

        GameObjectSave.sceneData.Add(Settings.persistentScene, sceneSave);
        return GameObjectSave;
    }

    public void ISaveableStoreScene(string sceneName)
    {
        //Nothing to do
    }
}
