using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MainMenuCanvas : MonoBehaviour
{
    public const string persistentSceneName = "PersistenceScene";


    public Image settingPanel;
    public Image mainMenuPanel;

    private void Start()
    {
        settingPanel.enabled = false;
        mainMenuPanel.enabled = true;
    }

    public void SwitchingToMainMenuPanel()
    {
        settingPanel.gameObject.SetActive(false);
        mainMenuPanel.gameObject.SetActive(true);
    }

    public void SwitchingToSettingPanel()
    {
        settingPanel.gameObject.SetActive(true);
        mainMenuPanel.gameObject.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadPersistentScene()
    {
        SceneManager.LoadScene(persistentSceneName);
    }
}
