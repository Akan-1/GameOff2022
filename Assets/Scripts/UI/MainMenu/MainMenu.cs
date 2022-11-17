using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject StartMenu;
    [SerializeField] private GameObject LevelsMenu;

    private void CloseAllMenus()
    {
        StartMenu.SetActive(false);
        LevelsMenu.SetActive(false);
    }

    public void OpenMenu(GameObject menu)
    {
        CloseAllMenus();
        menu.SetActive(true);
    }

    public void LoadScene(int index)
    {
        SceneLoader.LoadByIndex(index);
    }
}
