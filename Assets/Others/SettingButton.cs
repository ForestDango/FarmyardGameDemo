using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SettingButton : MonoBehaviour
{
    public Image SettingPanel;
    public Image mainMenuPanel;

    public void SwitchingToSettingPanel()
    {
        SettingPanel.enabled = true;
        mainMenuPanel.enabled = false;
    }
}
