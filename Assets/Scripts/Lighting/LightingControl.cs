using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering.Universal;

/// <summary>
/// 挂载在每一个有Light2D的游戏对象上，用于控制不同时期的光照强度
/// </summary>
public class LightingControl : MonoBehaviour
{
    //特定的光照不同时期
    [SerializeField] private LightingSchedule lightingSchedule;

    //是否开启光照闪烁
    [SerializeField] private bool isLightFlicker = false;
    [SerializeField] [Range(0f, 1f)] private float lightFlickerIntensity; //灯的闪烁力度
    [SerializeField] [Range(0f, 0.2f)] private float lightFlickerTimeMin;
    [SerializeField] [Range(0f, 0.2f)] private float lightFlickerTimeMax;

    private Light2D light2D;
    private Dictionary<string, float> lightingBrightnessDictionary = new Dictionary<string, float>(); //键为季节+小时， 值为lightingSchedule上的lightingBrtightnessArray数组
    private float currentLightIndensity; //记录当前的光照强度
    private float lightFlickerTimer = 0f; //计时器

    private Coroutine fadeInLightCoroutine;

    private void Awake()
    {
        light2D = GetComponentInChildren<Light2D>();

        if (light2D == null)
        {
            Debug.LogWarning("No Light2D Component Attacked To Object");
            enabled = false;
        }

        foreach (LightingBrtightness lightingBrtightness in lightingSchedule.lightingBrtightnessArray)
        {
            string key = lightingBrtightness.season.ToString() + lightingBrtightness.hour.ToString();

            lightingBrightnessDictionary.Add(key, lightingBrtightness.lightIndensity);
        }
    }

    private void OnEnable()
    {
        EventHandler.AdvanceGameHourEvent += EvnetHandler_AdvanceGameHourEvent;
        EventHandler.AfterSceneLoadEvent += EventHandler_AfterSceneLoadEvent;
    }

    private void OnDisable()
    {
        EventHandler.AdvanceGameHourEvent -= EvnetHandler_AdvanceGameHourEvent;
        EventHandler.AfterSceneLoadEvent -= EventHandler_AfterSceneLoadEvent;
    }
    /// <summary>
    /// 当场景加载完成后直接设置场景灯光强度
    /// </summary>

    private void EventHandler_AfterSceneLoadEvent()
    {
        SetLightingAfterSceneLoaded();
    }

    /// <summary>
    /// 每当游戏时间发生变化的时候就要设置灯光强度
    /// </summary>
    /// <param name="gameYear"></param>
    /// <param name="season"></param>
    /// <param name="gameDay"></param>
    /// <param name="gameOnWeek"></param>
    /// <param name="gameHour"></param>
    /// <param name="gameMinuter"></param>
    /// <param name="gameSecond"></param>
    private void EvnetHandler_AdvanceGameHourEvent(int gameYear, Season season, int gameDay, string gameOnWeek, int gameHour, int gameMinuter, int gameSecond)
    {
        SetLightingIndensity(season, gameHour, true);
    }

    private void Update()
    {
        if(isLightFlicker)
        {
            lightFlickerTimer -= Time.deltaTime;
        }

    }

    private void LateUpdate()
    {
        if(lightFlickerTimer <= 0f && isLightFlicker)
        {
            LightFlicker();
        }
        else
        {
            light2D.intensity = currentLightIndensity;
        }
    }
   

    private void SetLightingIndensity(Season gameSeason, int gameHour, bool fadeIn)
    {
        int i = 0;
        while (i <= 23)
        {
            string key = gameSeason.ToString() + (gameHour).ToString();

            if(lightingBrightnessDictionary.TryGetValue(key,out float targetLightingIntensity))
            {
                if (fadeIn)
                {
                    if (fadeInLightCoroutine != null)
                    {
                        StopCoroutine(fadeInLightCoroutine);
                    }
                        
                    fadeInLightCoroutine = StartCoroutine(FadeLightCoroutine(targetLightingIntensity));
                }
                else
                {
                    currentLightIndensity = targetLightingIntensity;
                }
                break;
            }

            i++;

            gameHour--;

            if(gameHour < 0)
            {
                gameHour = 23;
            }
        }
    }


    private IEnumerator FadeLightCoroutine(float targetLightingIntensity)
    {
        float fadeDuration = 5f;
        float fadeSpeed = Mathf.Abs(currentLightIndensity - targetLightingIntensity) / fadeDuration;

        while (!Mathf.Approximately(currentLightIndensity, targetLightingIntensity))
        {
            currentLightIndensity = Mathf.MoveTowards(currentLightIndensity, targetLightingIntensity, fadeSpeed * Time.deltaTime);

            yield return null;
        }
    }

    /// <summary>
    /// 当场景加载完成后直接设置场景灯光强度
    /// </summary>
    private void SetLightingAfterSceneLoaded()
    {
        Season gameSeason = TimeManager.Instance.GetGameSeason();
        int gameHour = TimeManager.Instance.GetGameTime().Hours;

        SetLightingIndensity(gameSeason, gameHour, false);
    }

    private void LightFlicker()
    {
        light2D.intensity = Random.Range(currentLightIndensity, currentLightIndensity + (currentLightIndensity * lightFlickerIntensity));

        lightFlickerTimer = Random.Range(lightFlickerTimeMin, lightFlickerTimeMax);
    }
}
