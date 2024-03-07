using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManiMenuButton : MonoBehaviour
{
    public Image SettingPanel;
    public Image mainMenuPanel;

    public void SwitchingToMainMenuPanel()
    {
        SettingPanel.enabled = false;
        mainMenuPanel.enabled = true;
    }
}
