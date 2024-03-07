using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(GenerateGUID))]
public class TimeManager : Singleton<TimeManager>,ISaveable
{
    private int gameYear = 1;
    private Season gameSeason = Season.Spring;
    private int gameDay = 1;
    private int gameHour = 6;
    private int gameMinute = 30;
    private int gameSecond = 0;
    private string gameOnWeek = "Mon";

    private bool gameClockPaused = false;

    private float gameTick = 0f;

    public string _iSaveableUniqueID;
    public string ISaveableUniqueID { get => _iSaveableUniqueID; set => _iSaveableUniqueID = value; }

    public GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get => _gameObjectSave; set => _gameObjectSave = value; }

    protected override void Awake()
    {
        base.Awake();

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }
    private void OnEnable()
    {
        ISaveableRegister();

        EventHandler.BeforeSceneUnloadEvent += BeforeSceneUploadFadeOut;
        EventHandler.AfterSceneLoadEvent += AfterSceneLoadFadeIn;
    }

    private void OnDisable()
    {
        ISaveableDeregister();

        EventHandler.BeforeSceneUnloadEvent -= BeforeSceneUploadFadeOut;
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoadFadeIn;
    }

    private void AfterSceneLoadFadeIn()
    {
        gameClockPaused = false;
    }

    private void BeforeSceneUploadFadeOut()
    {
        gameClockPaused = true;
    }

    private void Start()
    {
        EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameOnWeek, gameHour, gameMinute, gameSecond);
    }

    private void Update()
    {
        if (!gameClockPaused)
        {
            UpdateGameTick();
        }
    }

    private void UpdateGameTick()
    {
        gameTick += Time.deltaTime;
        if(gameTick >= Settings.secondTimePerGameTimeSecond)
        {
            gameTick -= Settings.secondTimePerGameTimeSecond;
        }

        UpdateGameSecond();
    }

    private void UpdateGameSecond()
    {
        gameSecond ++;
        if (gameSecond > 59)
        {
            gameSecond = 0;
            gameMinute++;
            if(gameMinute > 59)
            {
                gameMinute = 0;
                gameHour++;
                if(gameHour > 23)
                {
                    gameHour = 0;
                    gameDay++;

                    if(gameDay > 30)
                    {
                        gameDay = 1;

                        int gs = (int)gameSeason;
                        gs++;

                        gameSeason = (Season)gs;

                        if(gs > 3)
                        {
                            gs = 0;
                            gameSeason = (Season)gs;

                            gameYear++;
                            if (gameYear > 9999)
                                gameYear = 1;

                            EventHandler.CallAdvanceGameYearEvent(gameYear, gameSeason, gameDay, gameOnWeek, gameHour, gameMinute, gameSecond);
                        }
                        EventHandler.CallAdvanceGameDayEvent(gameYear, gameSeason, gameDay, gameOnWeek, gameHour, gameMinute, gameSecond);
                    }

                    gameOnWeek = GetDayOfWeek();
                    EventHandler.CallAdvanceGameDayEvent(gameYear, gameSeason, gameDay, gameOnWeek, gameHour, gameMinute, gameSecond);
                }

                EventHandler.CallAdvanceGameHourEvent(gameYear, gameSeason, gameDay, gameOnWeek, gameHour, gameMinute, gameSecond);
            }
            EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameOnWeek, gameHour, gameMinute, gameSecond);
        }
    }

    private string GetDayOfWeek()
    {
        int totalDays = ((int)gameSecond * 30) + gameDay;
        int dayOnWeek = totalDays % 7;

        switch (dayOnWeek)
        {
            case 1:
                return "Mon";
            case 2:
                return "Tue";
            case 3:
                return "Wed";
            case 4:
                return "Thu";
            case 5:
                return "Fri";
            case 6:
                return "Sat";
            case 0:
                return "Sun";
            default:
                return "";
        }
    }

    public TimeSpan GetGameTime()
    {
        TimeSpan gameTime = new TimeSpan(gameHour, gameMinute, gameSecond);
        return gameTime;
    }

    public Season GetGameSeason()
    {
        return gameSeason;
    }


    public void TestAdvanceGameMinute()
    {
        for (int i = 0; i < 60; i++)
        {
            UpdateGameSecond();
        }
    }

    public void TestAdvanceGameDay()
    {
        for (int i = 0; i < 86400; i++)
        {
            UpdateGameSecond();
        }
    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public GameObjectSave ISaveableSave()
    {
        GameObjectSave.sceneData.Remove(Settings.persistentScene);

        SceneSave sceneSave = new SceneSave();

        sceneSave.intDictionary = new Dictionary<string, int>();

        sceneSave.stringDictionary = new Dictionary<string, string>();

        sceneSave.intDictionary.Add("gameYear", gameYear);
        sceneSave.intDictionary.Add("gameDay", gameDay);
        sceneSave.intDictionary.Add("gameHour", gameHour);
        sceneSave.intDictionary.Add("gameMinute", gameMinute);
        sceneSave.intDictionary.Add("gameSecond", gameSecond);

        sceneSave.stringDictionary.Add("gameOnWeek", gameOnWeek);
        sceneSave.stringDictionary.Add("gameSeason", gameSeason.ToString());

        GameObjectSave.sceneData.Add(Settings.persistentScene, sceneSave);

        return GameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if(gameSave.gameObjectData.TryGetValue(ISaveableUniqueID,out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            if(GameObjectSave.sceneData.TryGetValue(Settings.persistentScene,out SceneSave sceneSave))
            {
                if(sceneSave.intDictionary!= null && sceneSave.stringDictionary != null)
                {
                    if(sceneSave.intDictionary.TryGetValue("gameYear",out int savedGameYear))
                    {
                        gameYear = savedGameYear;
                    }
                    if (sceneSave.intDictionary.TryGetValue("gameDay", out int savedGameDay))
                    {
                        gameDay = savedGameDay;
                    }
                    if (sceneSave.intDictionary.TryGetValue("gameHour", out int savedGameHour))
                    {
                        gameHour = savedGameHour;
                    }
                    if (sceneSave.intDictionary.TryGetValue("gameMinute", out int savedGameMinute))
                    {
                        gameMinute = savedGameMinute;
                    }
                    if (sceneSave.intDictionary.TryGetValue("gameSecond", out int savedGameSeconde))
                    {
                        gameSecond = savedGameSeconde;
                    }
                    if (sceneSave.stringDictionary.TryGetValue("gameOnWeek", out string savedGameOnWeek))
                    {
                        gameOnWeek = savedGameOnWeek;
                    }

                    if (sceneSave.stringDictionary.TryGetValue("gameSeason", out string savedGameSeason))
                    {
                        if(Enum.TryParse<Season>(savedGameSeason,out Season season))
                        {
                            gameSeason = season;
                        }
                    }

                    gameTick = 0f;

                    EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameOnWeek, gameHour, gameMinute, gameSecond);
                }
            }
        }
    }

    public void ISaveableStoreScene(string sceneName)
    {
        
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        
    }
}
