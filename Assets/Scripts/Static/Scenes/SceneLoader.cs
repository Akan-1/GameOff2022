using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public static int GetNextSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex + 1;
    }
    public static void LoadByIndex(int sceneIndex)
    {
        CharacterSwapper.Instance.CurrentPlayerController2D = null;
        CharacterSwapper.Instance.ClearCharacters();
        SceneManager.LoadScene(sceneIndex);
    }

    public static void NextScene()
    {
        CharacterSwapper.Instance.CurrentPlayerController2D = null;
        CharacterSwapper.Instance.ClearCharacters();
        LoadByIndex(GetNextSceneIndex());
    }

    public static void Reload()
    {
        CharacterSwapper.Instance.CurrentPlayerController2D = null;
        CharacterSwapper.Instance.ClearCharacters();
        LoadByIndex(SceneManager.GetActiveScene().buildIndex);
    }

    public static void Quit()
    {
        Application.Quit();
    }
}
