using UnityEngine;

/// <summary>
/// ��Ϸ���������������������Լ���Ϸ���к���Ļ�ߴ�
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
