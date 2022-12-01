using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    [SerializeField] private GameObject pausedMenuUI;

    void Start()
    {
        Resume();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }
    public void Resume()
    {
        GameIsPaused = false;
        pausedMenuUI.SetActive(false);
    }

    void Pause()
    {
        GameIsPaused = true;
        pausedMenuUI.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene(0);
    }

    public void Restart()
    {
        CharacterSwapper.Instance.CurrentPlayerController2D = null;
        CharacterSwapper.Instance.ClearCharacters();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("restart");
    }
}
