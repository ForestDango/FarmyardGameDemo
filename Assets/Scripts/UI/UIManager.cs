using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    private bool _pauseMenuOn = false;

    [SerializeField] private PauseMenuInventoryManagement pauseMenuInventoryManagement; 
    [SerializeField] private UIInventoryBar uIInventoryBar;
    [SerializeField] private GameObject pauseMenu = null;
    [SerializeField] private GameObject[] menuTabs = null;
    [SerializeField] private Button[] menuButtons = null;

    public bool PauseMenuOn
    {
        get => _pauseMenuOn;
        set => _pauseMenuOn = value;
    }

    protected override void Awake()
    {
        base.Awake();

        pauseMenu.SetActive(false);
    }

    private void Update()
    {
        UpdateMenu();
    }

    private void UpdateMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.LogFormat("Pause The Menu");
            if (PauseMenuOn)
            {
                DisablePauseMenu();
            }
            else
            {
                EnablePauseMenu();
            }
        }
    }

    public void EnablePauseMenu()
    {
        PauseMenuOn = true;
        PlayerController.Instance.PlayerInputDisabled = true ;
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        System.GC.Collect();

        HighlightButtonForSelectedTab();
    }

    private void HighlightButtonForSelectedTab()
    {
        for (int i = 0; i < menuButtons.Length; i++)
        {
            if (menuTabs[i].activeSelf)
            {
                SetButtonColorToActive(menuButtons[i]);
            }
            else
            {
                SetButtonColorToInactive(menuButtons[i]);
            }
        }
    }

    private void SetButtonColorToInactive(Button button)
    {
        ColorBlock colors = button.colors;

        colors.normalColor = colors.disabledColor;

        button.colors = colors;
    }

    private void SetButtonColorToActive(Button button)
    {
        ColorBlock colors = button.colors;

        colors.normalColor = colors.pressedColor;

        button.colors = colors;
    }

    public void DisablePauseMenu()
    {
        pauseMenuInventoryManagement.DestroyCurrentlyDraggedItem();

        uIInventoryBar.DesotryCurrentlyDraggedItem();
        uIInventoryBar.ClearCurrentlyDraggedItem();
        PauseMenuOn = false;
        PlayerController.Instance.PlayerInputDisabled = false ;
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
    }

    public void SwitchPauseMenuTab(int tabNum)
    {
        for (int i = 0; i < menuTabs.Length; i++)
        {
            if(i != tabNum)
            {
                menuTabs[i].SetActive(false);
            }
            else
            {
                menuTabs[i].SetActive(true);
            }
        }

        HighlightButtonForSelectedTab();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
