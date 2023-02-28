using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] GameObject mainTab;
    [SerializeField] GameObject statsTab;
    [SerializeField] GameObject settingsTab;

    List<GameObject> tabs;

    public enum menuState
    {
        MeinTab,
        PlayOnline,
        PlayOffline,
        StatsTab,
        SettingsTab,
        Back,
        Quit
    }

    private void Awake()
    {
        //Add tabs to tabs list
        tabs.Add(mainTab);
        tabs.Add(statsTab);
        tabs.Add(settingsTab);
    }

    //Switch tabs or do action when button is pressed
    public void SwitchMenuState(menuState state)
    {
        CloseAllTabs();

        switch (state)
        {
            case menuState.MeinTab:
                break;

            case menuState.PlayOnline:
                SceneManager.LoadScene("GameField");
                break;

            case menuState.PlayOffline:
                SceneManager.LoadScene("GameField");
                break;

            case menuState.StatsTab:
                statsTab.SetActive(true);
                break;

            case menuState.SettingsTab:
                settingsTab.SetActive(true);
                break;

            case menuState.Back:
                mainTab.SetActive(true);
                break;

            case menuState.Quit:
                break;
        }
    }

    //Helpfull functions

    //Close all tabs
    public void CloseAllTabs()
    {
        foreach (GameObject menu in tabs)
        {
            menu.SetActive(false);
        }
    }



    //Events

    //Start games
    public void PlayOnline()
    {
        SwitchMenuState(menuState.PlayOnline);
    }
    public void PlayOffline()
    {
        SwitchMenuState(menuState.PlayOffline);
    }


    //Open tabs
    public void OpenStats()
    {
        SwitchMenuState(menuState.StatsTab);
    }
    public void OpenSettings()
    {
        SwitchMenuState(menuState.SettingsTab);
    }
    public void Back()
    {
        SwitchMenuState(menuState.Back);
    }

    //Leave game
    public void Quit()
    {
        Application.Quit();
    }
}
