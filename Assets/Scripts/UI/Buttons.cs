using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    public void StartGame(int index)
    {
        SceneManager.LoadScene(index);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}
