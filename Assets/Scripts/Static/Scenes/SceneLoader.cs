using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;
    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }

        Destroy(this);
    }

    public void LoadByIndex(int sceneIndex)
    {
        CharacterSwapper.Instance.ClearCharacters();
        SceneManager.LoadScene(sceneIndex);
    }

    public void LoadNext()
    {
        CharacterSwapper.Instance.ClearCharacters();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
