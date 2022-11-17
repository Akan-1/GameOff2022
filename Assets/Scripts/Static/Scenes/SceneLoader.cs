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
        CharacterSwapper.Instance.ClearCharacters();
        SceneManager.LoadScene(sceneIndex);
    }

    public static void NextScene()
    {
        CharacterSwapper.Instance.ClearCharacters();
        SceneLoader.LoadByIndex(GetNextSceneIndex());
    }

    public static void Quit()
    {
        Application.Quit();
    }
}
