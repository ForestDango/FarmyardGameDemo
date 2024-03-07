using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(GenerateGUID))]

public class GridPropertiesManager : Singleton<GridPropertiesManager>, ISaveable//继承ISaveable并实现方法
{
    private bool isFirstTimeSceneLoaded = true;
    private Transform eggsParentTransform;
    private Transform cropParentTransform;
    [HideInInspector]public Grid grid;
    private Tilemap groundDecoration1;
    private Tilemap groundDecoration2;
    public Tile[] dugGround = null;
    public Tile[] waterGround = null;
    private Dictionary<string, GridPropertyDetails> gridPropertyDictionary; //GridPropertyDetails的字典
    [SerializeField] private SO_GridProperties[] so_GridPropertiesArray = null; //创建的每个场景中合起来的数组
    [SerializeField] private SO_CropDetailsList so_CropDetailsList = null;
    [SerializeField] private SO_EggDetailsList so_EggDetailsList = null;

    private string iSaveableUniqueID; //GUID
    public string ISaveableUniqueID { get => iSaveableUniqueID; set => iSaveableUniqueID = value; }

    private GameObjectSave gameObjectSave; //scneneData
    public GameObjectSave GameObjectSave { get => gameObjectSave; set => gameObjectSave = value; }

    protected override void Awake()
    {
        base.Awake();

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void Start()
    {
        IntializeGridProperty();
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += AfterSceneLoaded;
        EventHandler.AdvanceGameDayEvent += AdvanceDay;
        ISaveableRegister();
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoaded;
        EventHandler.AdvanceGameDayEvent -= AdvanceDay;
        ISaveableDeregister();
    }

    

    private void ClearDisplayGroundDecorations()
    {
        groundDecoration1.ClearAllTiles();
        groundDecoration2.ClearAllTiles();
    }

    private void ClearDisplayAllPlantedCrops()
    {
        Crops[] cropArray;
        cropArray = FindObjectsOfType<Crops>();

        foreach (Crops crops in cropArray)
        {
            Destroy(crops.gameObject);
        }
    }
    /// <summary>
    /// 清除所有正在孵化的鸡蛋
    /// </summary>
    private void ClearDisplayAllHatchedEggs()
    {
        Eggs[] eggsArray;
        eggsArray = FindObjectsOfType<Eggs>();

        foreach (Eggs eggs in eggsArray)
        {
            Destroy(eggs.gameObject);
        }
    }

    private void ClearDisplayGridPropertyDetails()
    {
        ClearDisplayGroundDecorations();

        ClearDisplayAllPlantedCrops();

        ClearDisplayAllHatchedEggs();
    }

    public void DisplayDugGround(GridPropertyDetails gridPropertyDetails)
    {
        if(gridPropertyDetails.daysSinceDug > -1)
        {
            ConnectDugGround(gridPropertyDetails);
        }
    }

    public void DisplayWaterGround(GridPropertyDetails gridPropertyDetails)
    {
        if (gridPropertyDetails.daysSinceWatered > -1)
        {
            ConnectWateredGround(gridPropertyDetails);
        }
    }

    private void ConnectDugGround(GridPropertyDetails gridPropertyDetails)
    {
        Tile dugTile0 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
        groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0), dugTile0);

        //判断该tile上下左右四个格子
        GridPropertyDetails adjacentGridPropertyDetails;

        adjacentGridPropertyDetails = GetGridPropetyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
        if(adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile1 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY+1);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1, 0), dugTile1);
        }

        adjacentGridPropertyDetails = GetGridPropetyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile2 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1, 0), dugTile2);
        }

        adjacentGridPropertyDetails = GetGridPropetyDetails(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile3 = SetDugTile(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY) ;
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY, 0), dugTile3);
        }

        adjacentGridPropertyDetails = GetGridPropetyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile4 = SetDugTile(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY, 0), dugTile4);
        }

    }

    private void ConnectWateredGround(GridPropertyDetails gridPropertyDetails)
    {
        Tile waterTile0 = SetWaterTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
        groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0), waterTile0);

        //判断该tile上下左右四个格子
        GridPropertyDetails adjacentGridPropertyDetails;

        adjacentGridPropertyDetails = GetGridPropetyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceWatered > -1)
        {
            Tile waterTile1 = SetWaterTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1, 0), waterTile1);
        }

        adjacentGridPropertyDetails = GetGridPropetyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceWatered > -1)
        {
            Tile waterTile2 = SetWaterTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1, 0), waterTile2);
        }

        adjacentGridPropertyDetails = GetGridPropetyDetails(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceWatered > -1)
        {
            Tile waterTile3 = SetWaterTile(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY, 0), waterTile3);
        }

        adjacentGridPropertyDetails = GetGridPropetyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceWatered > -1)
        {
            Tile waterTile4 = SetWaterTile(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY, 0), waterTile4);
        }

    }

    private Tile SetWaterTile(int gridX, int gridY)
    {
        bool upWatered = IsGridSquareWatered(gridX, gridY + 1);
        bool downWatered = IsGridSquareWatered(gridX, gridY - 1);
        bool leftWatered = IsGridSquareWatered(gridX - 1, gridY);
        bool rightWatered = IsGridSquareWatered(gridX + 1, gridY);

        if (!upWatered && !downWatered && !rightWatered && !leftWatered)
        {
            return waterGround[0];
        }
        else if (!upWatered && downWatered && rightWatered && !leftWatered)
        {
            return waterGround[1];
        }
        else if (!upWatered && downWatered && rightWatered && leftWatered)
        {
            return waterGround[2];
        }
        else if (!upWatered && downWatered && !rightWatered && leftWatered)
        {
            return waterGround[3];
        }
        else if (!upWatered && downWatered && !rightWatered && !leftWatered)
        {
            return waterGround[4];
        }
        else if (upWatered && downWatered && rightWatered && !leftWatered)
        {
            return waterGround[5];
        }
        else if (upWatered && downWatered && rightWatered && leftWatered)
        {
            return waterGround[6];
        }
        else if (upWatered && downWatered && !rightWatered && leftWatered)
        {
            return waterGround[7];
        }
        else if (upWatered && downWatered && !rightWatered && !leftWatered)
        {
            return waterGround[8];
        }
        else if (upWatered && !downWatered && rightWatered && !leftWatered)
        {
            return waterGround[9];
        }
        else if (upWatered && !downWatered && rightWatered && leftWatered)
        {
            return waterGround[10];
        }
        else if (upWatered && !downWatered && !rightWatered && leftWatered)
        {
            return waterGround[11];
        }
        else if (upWatered && !downWatered && !rightWatered && !leftWatered)
        {
            return waterGround[12];
        }
        else if (!upWatered && !downWatered && rightWatered && !leftWatered)
        {
            return waterGround[13];
        }
        else if (!upWatered && !downWatered && rightWatered && leftWatered)
        {
            return waterGround[14];
        }
        else if (!upWatered && !downWatered && !rightWatered && leftWatered)
        {
            return waterGround[15];
        }

        return null;
    }

    private bool IsGridSquareWatered(int gridX, int gridY)
    {
        GridPropertyDetails gridPropertyDetails = GetGridPropetyDetails(gridX, gridY);

        if(gridPropertyDetails == null)
        {
            return false;
        }
        else if(gridPropertyDetails.daysSinceWatered > -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private Tile SetDugTile(int gridX, int gridY)
    {
        bool upDug = IsGridSquareDug(gridX, gridY + 1);
        bool downDug = IsGridSquareDug(gridX, gridY - 1);
        bool leftDug = IsGridSquareDug(gridX - 1, gridY);
        bool rightDug = IsGridSquareDug(gridX + 1, gridY);

        if(!upDug && !downDug && !rightDug && !leftDug)
        {
            return dugGround[0];
        }
        else if (!upDug && downDug && rightDug && !leftDug)
        {
            return dugGround[1];
        }
        else if (!upDug && downDug && rightDug && leftDug)
        {
            return dugGround[2];
        }
        else if (!upDug && downDug && !rightDug && leftDug)
        {
            return dugGround[3];
        }
        else if (!upDug && downDug && !rightDug && !leftDug)
        {
            return dugGround[4];
        }
        else if (upDug && downDug && rightDug && !leftDug)
        {
            return dugGround[5];
        }
        else if (upDug && downDug && rightDug && leftDug)
        {
            return dugGround[6];
        }
        else if (upDug && downDug && !rightDug && leftDug)
        {
            return dugGround[7];
        }
        else if (upDug && downDug && !rightDug && !leftDug)
        {
            return dugGround[8];
        }
        else if (upDug && !downDug && rightDug && !leftDug)
        {
            return dugGround[9];
        }
        else if (upDug && !downDug && rightDug && leftDug)
        {
            return dugGround[10];
        }
        else if (upDug && !downDug && !rightDug && leftDug)
        {
            return dugGround[11];
        }
        else if (upDug && !downDug && !rightDug && !leftDug)
        {
            return dugGround[12];
        }
        else if (!upDug && !downDug && rightDug && !leftDug)
        {
            return dugGround[13];
        }
        else if (!upDug && !downDug && rightDug && leftDug)
        {
            return dugGround[14];
        }
        else if (!upDug && !downDug && !rightDug && leftDug)
        {
            return dugGround[15];
        }

        return null;
    }

    private bool IsGridSquareDug(int gridX, int gridY)
    {
        GridPropertyDetails gridPropertyDetails = GetGridPropetyDetails(gridX, gridY);

        if(gridPropertyDetails == null)
        {
            return false;
        }
        else if(gridPropertyDetails.daysSinceDug > -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 当AdvanceDay游戏进入下一天的时候以及重新存储场景的时候调用
    /// </summary>
    private void DisplayGridPropertyDetails()
    {
        foreach (KeyValuePair<string,GridPropertyDetails> item in gridPropertyDictionary)
        {
            GridPropertyDetails gridPropertyDetails = item.Value;

            DisplayDugGround(gridPropertyDetails);

            DisplayWaterGround(gridPropertyDetails);

            DisplayPlantedCrop(gridPropertyDetails);

            DisplayHatchedEggs(gridPropertyDetails);
        }
    }

    /// <summary>
    /// 展示种植后的植物
    /// </summary>
    /// <param name="gridPropertyDetails"></param>
    public void DisplayPlantedCrop(GridPropertyDetails gridPropertyDetails)
    {
       if(gridPropertyDetails.seedItemCode > -1)
        {
            CropDetails cropDetails = so_CropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);

            if (cropDetails != null)
            {

                GameObject cropPrefab;

                int growthStages = cropDetails.growthDays.Length;
                int currentGrowthStage = 0;


                //假如最后阶段的成长时间为2天，总的成长时间growthdays为5，那么到第三天的时候current就会变成i的值
                for (int i = growthStages - 1; i >= 0; i--)
                {
                    if (gridPropertyDetails.growthDays >= cropDetails.growthDays[i])
                    {
                        currentGrowthStage = i;
                        break;
                    }

                }

                cropPrefab = cropDetails.growthPrefab[currentGrowthStage];

                Sprite growSprite = cropDetails.growthSprite[currentGrowthStage];

                Vector3 worldPosition = groundDecoration2.CellToWorld(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0));

                worldPosition = new Vector3(worldPosition.x + Settings.gridCellSize / 2f, worldPosition.y, worldPosition.z);

                GameObject cropInstance = Instantiate(cropPrefab, worldPosition, Quaternion.identity);

                cropInstance.transform.GetComponentInChildren<SpriteRenderer>().sprite = growSprite;
                cropInstance.transform.SetParent(cropParentTransform);
                cropInstance.GetComponent<Crops>().cropGridPosition = new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
            }
        }
    }
    public void DisplayHatchedEggs(GridPropertyDetails gridPropertyDetails)
    {
        if (gridPropertyDetails.eggItemCode > -1)
        {
            EggDetails eggDetails = so_EggDetailsList.GetEggDetails(gridPropertyDetails.eggItemCode); //先通过eggItemCode获取eggDetails

            if (eggDetails != null)
            {
                Debug.Log("成功找到eggDetails");
                GameObject eggPrefab;

                int growthStages = eggDetails.growthDays.Length;

                int currentGrowthStage = 0;

                //获取egg的预制体以及当前成长阶段
                //只需要根据成长阶段就能知道当前成长阶段
                for (int i = growthStages - 1; i >= 0; i--)
                {
                    if (gridPropertyDetails.growthDays >= eggDetails.growthDays[i])
                    {
                        currentGrowthStage = i;
                        break;
                    }

                }
                //通多当前成长阶段获取Egg的预制体
                eggPrefab = eggDetails.growthPrefab[currentGrowthStage];
                //通多当前成长阶段获得Egg的精灵图
                Sprite growSprite = eggDetails.growthSprite[currentGrowthStage];
                //获取Egg的世界坐标
                Vector3 worldPosition = groundDecoration2.CellToWorld(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0));

                worldPosition = new Vector3(worldPosition.x + Settings.gridCellSize / 2f, worldPosition.y, worldPosition.z);
                //生成Egg的游戏对象
                GameObject eggInstance = Instantiate(eggPrefab, worldPosition, Quaternion.identity);
                //添加Egg游戏对象的精灵图，父物体，以及再Grid中的位置
                eggInstance.transform.GetComponentInChildren<SpriteRenderer>().sprite = growSprite;
                eggInstance.transform.SetParent(eggsParentTransform);
                eggInstance.GetComponent<Eggs>().eggGridPosition = new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
            }
        }
    }
    private void AdvanceDay(int gameYear, Season season, int gameDay, string gameOnWeek, int gameHour, int gameMinuter, int gameSecond)
    {
        ClearDisplayGridPropertyDetails();

        foreach (SO_GridProperties so_GridProperties in so_GridPropertiesArray)
        {
            if(GameObjectSave.sceneData.TryGetValue(so_GridProperties.sceneName.ToString(),out SceneSave sceneSave))
            {
                if(sceneSave.gridPropetyDeatilsDictionary != null)
                {
                    for (int i = sceneSave.gridPropetyDeatilsDictionary.Count -1; i >= 0; i--)
                    {
                        KeyValuePair<string, GridPropertyDetails> item = sceneSave.gridPropetyDeatilsDictionary.ElementAt(i);

                        GridPropertyDetails gridPropertyDetails = item.Value;

                        if(gridPropertyDetails.growthDays > -1)
                        {
                            gridPropertyDetails.growthDays += 1;
                        }

                        if(gridPropertyDetails.daysSinceWatered > -1)
                        {
                            gridPropertyDetails.daysSinceWatered = -1;
                        }

                        if(gridPropertyDetails.daysSinceHatched > -1)
                        {
                            gridPropertyDetails.daysSinceHatched += 1;
                        }

                        SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails, sceneSave.gridPropetyDeatilsDictionary);


                    }
                }
            }
        }

        DisplayGridPropertyDetails();
    }

    private void AfterSceneLoaded()
    {
        if(GameObject.FindGameObjectWithTag(Tags.cropsParentTransform) != null)
        {
            cropParentTransform = GameObject.FindGameObjectWithTag(Tags.cropsParentTransform).transform;
            eggsParentTransform = GameObject.FindGameObjectWithTag(Tags.cropsParentTransform).transform;
        }
        else
        {
            eggsParentTransform = null;
            cropParentTransform = null;
        }

        grid = GameObject.FindObjectOfType<Grid>();

        groundDecoration1 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration1).GetComponent<Tilemap>();
        groundDecoration2 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration2).GetComponent<Tilemap>();
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveableRestoreScene(string sceneName)
    {
       if(GameObjectSave.sceneData.TryGetValue(sceneName,out SceneSave sceneSave))
        {
            if(sceneSave.gridPropetyDeatilsDictionary != null)
            {
                gridPropertyDictionary = sceneSave.gridPropetyDeatilsDictionary;
            }

            if(sceneSave.boolDictionary != null && sceneSave.boolDictionary.TryGetValue("isFirstTimeSceneLoaded",out bool storedIsFirstTimeSceneLoaded))
            {
                isFirstTimeSceneLoaded = storedIsFirstTimeSceneLoaded;

            }

            if (isFirstTimeSceneLoaded)
                EventHandler.CallInstantiateCropPrefabEvent();

            if(gridPropertyDictionary.Count > 0)
            {
                ClearDisplayGridPropertyDetails();

                DisplayGridPropertyDetails();
            }

            if (isFirstTimeSceneLoaded)
                isFirstTimeSceneLoaded = false;
        }
    }

    public void ISaveableStoreScene(string sceneName)
    {
        GameObjectSave.sceneData.Remove(sceneName);

        SceneSave sceneSave = new SceneSave();

        sceneSave.gridPropetyDeatilsDictionary = gridPropertyDictionary;

        sceneSave.boolDictionary = new Dictionary<string, bool>();
        sceneSave.boolDictionary.Add("isFirstTimeSceneLoaded", isFirstTimeSceneLoaded); 

        GameObjectSave.sceneData.Add(sceneName, sceneSave); 
    }

    /// <summary>
    /// 初始化场景的中的GridProperty
    /// </summary>
    private void IntializeGridProperty()
    {
        foreach (SO_GridProperties so_GridProperties in so_GridPropertiesArray) //先遍历场景数据结构的数组
        {
            Dictionary<string, GridPropertyDetails> gridPropertyDictionary = new Dictionary<string, GridPropertyDetails>(); //对于每个场景创建一个字典，string即GUID

            foreach (GridPropetry gridPropetry in so_GridProperties.gridPropertyList) //遍历场景数据结构中的链表元素
            {
                GridPropertyDetails gridPropertyDetails;

                gridPropertyDetails = GetGridPropetyDetails(gridPropetry.gridCoordinate.x, gridPropetry.gridCoordinate.y,gridPropertyDictionary);//用坐标来初始化gridPropertyDetails

                if (gridPropertyDetails == null) //如果找不到就创建一个新的
                {
                    gridPropertyDetails = new GridPropertyDetails();
                }

                //用switch来判断gridPropetry的gridBoolProperty枚举
                //以此来给gridPropertyDetails上的对应bool来赋值
                switch (gridPropetry.gridBoolProperty)
                {
                    case GridBoolProperty.diggable:
                        gridPropertyDetails.isDiggable = gridPropetry.gridBoolValue;
                        break;
                    case GridBoolProperty.canDropItem:
                        gridPropertyDetails.canDropItem = gridPropetry.gridBoolValue;
                        break;
                    case GridBoolProperty.canPlaneFurniture:
                        gridPropertyDetails.canPlaceFurntiure = gridPropetry.gridBoolValue;
                        break;
                    case GridBoolProperty.isPath:
                        gridPropertyDetails.isPath = gridPropetry.gridBoolValue;
                        break;
                    case GridBoolProperty.isNPCObstacle:
                        gridPropertyDetails.isNPCObstacle = gridPropetry.gridBoolValue;
                        break;
                    case GridBoolProperty.canHatchEggs:
                        gridPropertyDetails.canHatchEggs = gridPropetry.gridBoolValue;
                        break;
                    default:
                        break;
                }

                SetGridPropertyDetails(gridPropetry.gridCoordinate.x,gridPropetry.gridCoordinate.y,gridPropertyDetails,gridPropertyDictionary);
            }

            SceneSave sceneSave = new SceneSave();

            sceneSave.gridPropetyDeatilsDictionary = gridPropertyDictionary;//吧我们新建的gridPropertyDictionary复制给sceneSave的字典

            if (so_GridProperties.sceneName .ToString() == SceneControllerManager.Instance.startingSceneName.ToString())
            {
                this.gridPropertyDictionary = gridPropertyDictionary;
            }

            sceneSave.boolDictionary = new Dictionary<string, bool>();
            sceneSave.boolDictionary.Add("isFirstTimeSceneLoaded",true);

            GameObjectSave.sceneData.Add(so_GridProperties.sceneName.ToString(), sceneSave);
        }
    }
    /// <summary>
    /// 重载函数，用默认的gridPropertyDictionary来储值
    /// </summary>
    /// <param name="gridX"></param>
    /// <param name="gridY"></param>
    /// <param name="gridPropertyDetails"></param>
    public void SetGridPropertyDetails(int gridX,int gridY, GridPropertyDetails gridPropertyDetails)
    {
        SetGridPropertyDetails(gridX, gridY, gridPropertyDetails, gridPropertyDictionary);
    }

    private void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails, Dictionary<string, GridPropertyDetails> gridPropertyDictionary)
    {
        string key = "x" + gridX + "y" + gridY;

        gridPropertyDetails.gridX = gridX;
        gridPropertyDetails.gridY = gridY;

        gridPropertyDictionary[key] = gridPropertyDetails;
    }

    public Crops GetCropObjectAtGridPosition(GridPropertyDetails gridPropertyDetails)
    {
        Vector3 worldPosition = grid.GetCellCenterWorld(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0));
        Collider2D[] collidet2DArray = Physics2D.OverlapPointAll(worldPosition);

        Crops crop = null;

        for (int i = 0; i < collidet2DArray.Length; i++)
        {
            crop = collidet2DArray[i].gameObject.GetComponentInChildren<Crops>();
            if (crop != null && crop.cropGridPosition == new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY))
                break;
            crop = collidet2DArray[i].gameObject.GetComponentInParent<Crops>();
            if (crop != null && crop.cropGridPosition == new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY))
                break;
        }

        return crop;
    }

    public Eggs GetEggsObjectAtGridPosition(GridPropertyDetails gridPropertyDetails)
    {
        Vector3 worldPosition = grid.GetCellCenterWorld(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0));
        Collider2D[] collidet2DArray = Physics2D.OverlapPointAll(worldPosition);

        Eggs eggs = null;

        for (int i = 0; i < collidet2DArray.Length; i++)
        {
            eggs = collidet2DArray[i].gameObject.GetComponentInChildren<Eggs>();
            if (eggs != null && eggs.eggGridPosition == new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY))
                break;
            eggs = collidet2DArray[i].gameObject.GetComponentInParent<Eggs>();
            if (eggs != null && eggs.eggGridPosition == new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY))
                break;
        }

        return eggs;
    }


    public GridPropertyDetails GetGridPropetyDetails(int gridX, int gridY,Dictionary<string , GridPropertyDetails> gridPropertyDictionary)
    {
        string key = "x" + gridX + "y" + gridY;

        GridPropertyDetails gridPropertyDetails;

        if(!gridPropertyDictionary.TryGetValue(key, out gridPropertyDetails))
        {
            return null;
        }
        else
        {
            return gridPropertyDetails;
        }
    }

    /// <summary>
    /// 重载函数，也是用默认的gridPropertyDictionary来取值
    /// </summary>
    /// <param name="gridX"></param>
    /// <param name="gridY"></param>
    /// <returns></returns>

    public GridPropertyDetails GetGridPropetyDetails(int gridX, int gridY)
    {
        return GetGridPropetyDetails(gridX, gridY, gridPropertyDictionary);
    }

    public bool  GetGridDimensions(SceneName sceneName,out Vector2Int gridDimension,out Vector2Int gridOrigin)
    {
        gridDimension = Vector2Int.zero;
        gridOrigin = Vector2Int.zero;

        foreach (SO_GridProperties so_GridProperties in so_GridPropertiesArray)
        {
            if (so_GridProperties.sceneName == sceneName)
            {
                gridDimension.x = so_GridProperties.gridWidth;
                gridDimension.y = so_GridProperties.gridHeight;

                gridOrigin.x = so_GridProperties.originX;
                gridOrigin.y = so_GridProperties.originY;

                return true;
            }
        }

        return false;
    }

    public CropDetails GetCropDetails(int itemCode)
    {
        return so_CropDetailsList.GetCropDetails(itemCode);
    }

    public EggDetails GetEggDetails(int itemCode)
    {
        return so_EggDetailsList.GetEggDetails(itemCode);
    }

    public GameObjectSave ISaveableSave()
    {
        ISaveableStoreScene(SceneManager.GetActiveScene().name);

        return GameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if(gameSave.gameObjectData.TryGetValue(ISaveableUniqueID,out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            ISaveableRestoreScene(SceneManager.GetActiveScene().name);
        }
    }
}
