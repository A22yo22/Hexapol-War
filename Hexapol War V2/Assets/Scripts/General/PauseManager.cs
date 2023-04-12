using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu;

    bool pauseMenuOpen = false;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (!pauseMenuOpen)
            {
                OpenPauseMenu();
            }
            else
            {
                ClosePauseMenu();
            }
        }
    }

    public void OpenPauseMenu()
    {
        pauseMenuOpen = true;

        pauseMenu.SetActive(true);

        foreach (PlayerInteractions player in GameDataHolder.instance.players)
        {
            player.enabled = false;
        }
    }

    public void ClosePauseMenu()
    {
        pauseMenuOpen = false;

        pauseMenu.SetActive(false);

        foreach (PlayerInteractions player in GameDataHolder.instance.players)
        {
            player.enabled = true;
        }
    }

    public void LeaveServer()
    {
        NetworkManager.singleton.StopHost();
        NetworkManager.singleton.StopClient();

        Destroy(NetworkManager.singleton.gameObject);

        SceneManager.LoadScene("MainMenu");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
