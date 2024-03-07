using UnityEngine;

/// <summary>
/// 游戏管理器，用来管理天气以及游戏发行后屏幕尺寸
/// </summary>
public class GameManager : Singleton<GameManager>
{

    public Weather currentWeather;
    protected override void Awake()
    {
        base.Awake();

        currentWeather = Weather.dry;

        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow, 0);
    }

}
