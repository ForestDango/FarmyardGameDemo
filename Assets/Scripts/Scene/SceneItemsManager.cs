using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(GenerateGUID))] //需要组件GenerateGUID
public class SceneItemsManager : Singleton<SceneItemsManager>, ISaveable //继承ISaveable并实现方法
{
    private Transform parentItem;
    [SerializeField] private GameObject itemPrefab = null;

    private string _iSaveableUniqueId;

    public string ISaveableUniqueID { get => _iSaveableUniqueId; set => _iSaveableUniqueId = value; } //GUID属性，每帧更新

    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get => _gameObjectSave; set => _gameObjectSave = value; } //scneneData属性，更新

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
    /// 在AfterSceneLoadEvent触发后，才能执行parentItem的实例化
    /// </summary>
    public void AfterSceneLoaded()
    {
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
    }

    /// <summary>
    /// 销毁所有场景中的物品
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
    /// 重新加载场景中的所有物品，先销毁所有物品游戏对象再初始化
    /// </summary>
    /// <param name="sceneName"></param>
    public void ISaveableRestoreScene(string sceneName)
    {
        if(GameObjectSave.sceneData.TryGetValue(sceneName,out SceneSave sceneSave))//找到特定的sceneName字符串，如果找到了就销毁该场景的
        {
            if(sceneSave.listSceneItem!= null )
            {
                DestorySceneItems();

                InstantiateSceneItems(sceneSave.listSceneItem);
            }
        }
    }

    /// <summary>
    /// 初始化SceneItem链表，并给所有Item重新赋值
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
    /// 将场景中的所有Item存储起来
    /// </summary>
    /// <param name="sceneName"></param>
    public void ISaveableStoreScene(string sceneName)
    {
        GameObjectSave.sceneData.Remove(sceneName); //先清空该场景

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
    /// 移除SaveStoreManager.Instance.iSaveableObjectLists链表的该数据
    /// </summary>
    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }
    /// <summary>
    /// 添加SaveStoreManager.Instance.iSaveableObjectLists链表的该数据
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
