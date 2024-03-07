using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(GenerateGUID))] //��Ҫ���GenerateGUID
public class SceneItemsManager : Singleton<SceneItemsManager>, ISaveable //�̳�ISaveable��ʵ�ַ���
{
    private Transform parentItem;
    [SerializeField] private GameObject itemPrefab = null;

    private string _iSaveableUniqueId;

    public string ISaveableUniqueID { get => _iSaveableUniqueId; set => _iSaveableUniqueId = value; } //GUID���ԣ�ÿ֡����

    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get => _gameObjectSave; set => _gameObjectSave = value; } //scneneData���ԣ�����

    protected override void Awake()
    {
        base.Awake();

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += AfterSceneLoaded;
        ISaveableRegister();
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoaded;
        ISaveableDeregister();
    }

    /// <summary>
    /// ��AfterSceneLoadEvent�����󣬲���ִ��parentItem��ʵ����
    /// </summary>
    public void AfterSceneLoaded()
    {
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
    }

    /// <summary>
    /// �������г����е���Ʒ
    /// </summary>
    private void DestorySceneItems()
    {
        Item[] itemInScenes = GameObject.FindObjectsOfType<Item>();

        for (int i = itemInScenes.Length -1; i > -1; i--)
        {
            Destroy(itemInScenes[i].gameObject);
        }
    }

    /// <summary>
    /// ���¼��س����е�������Ʒ��������������Ʒ��Ϸ�����ٳ�ʼ��
    /// </summary>
    /// <param name="sceneName"></param>
    public void ISaveableRestoreScene(string sceneName)
    {
        if(GameObjectSave.sceneData.TryGetValue(sceneName,out SceneSave sceneSave))//�ҵ��ض���sceneName�ַ���������ҵ��˾����ٸó�����
        {
            if(sceneSave.listSceneItem!= null )
            {
                DestorySceneItems();

                InstantiateSceneItems(sceneSave.listSceneItem);
            }
        }
    }

    /// <summary>
    /// ��ʼ��SceneItem������������Item���¸�ֵ
    /// </summary>
    /// <param name="sceneItemList"></param>
    private void InstantiateSceneItems(List<SceneItem> sceneItemList)
    {
        GameObject itemGameObject;
        foreach (SceneItem sceneItem in sceneItemList)
        {
            itemGameObject = Instantiate(itemPrefab, new Vector3(sceneItem.position.x, sceneItem.position.y, sceneItem.position.z), Quaternion.identity, parentItem);

            Item item = itemGameObject.GetComponent<Item>();

            item.ItemCode = sceneItem.itemCode;
            item.name = sceneItem.itemName;

        }
    }

    public void InstantiateSceneItem(int itemCode,Vector3 itemPosition)
    {
        GameObject itemGameObject = Instantiate(itemPrefab, itemPosition, Quaternion.identity, parentItem);
        Item item = itemGameObject.GetComponent<Item>();
        item.Init(itemCode);        
    }

    /// <summary>
    /// �������е�����Item�洢����
    /// </summary>
    /// <param name="sceneName"></param>
    public void ISaveableStoreScene(string sceneName)
    {
        GameObjectSave.sceneData.Remove(sceneName); //����ոó���

        List<SceneItem> sceneItemLists = new List<SceneItem>();
        Item[] itemsInScene = FindObjectsOfType<Item>();

        foreach (Item item in itemsInScene)
        {
            SceneItem sceneItem = new SceneItem();
            sceneItem.itemCode = item.ItemCode;
            sceneItem.position = new Vector3Serializable(item.transform.position.x, item.transform.position.y, item.transform.position.z);
            sceneItem.itemName = item.name;

            sceneItemLists.Add(sceneItem);
        }

        SceneSave sceneSave = new SceneSave();
        sceneSave.listSceneItem = sceneItemLists;

        GameObjectSave.sceneData.Add(sceneName, sceneSave);
    }

    /// <summary>
    /// �Ƴ�SaveStoreManager.Instance.iSaveableObjectLists����ĸ�����
    /// </summary>
    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }
    /// <summary>
    /// ���SaveStoreManager.Instance.iSaveableObjectLists����ĸ�����
    /// </summary>
    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public GameObjectSave ISaveableSave()
    {
        ISaveableStoreScene(SceneManager.GetActiveScene().name);

        return GameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if (gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            ISaveableRestoreScene(SceneManager.GetActiveScene().name);
        }
    }
}
