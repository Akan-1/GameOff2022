using UnityEngine;

public class NextLevelTrigger : MonoBehaviour
{
    [SerializeField] private int playersNeededToCompleteLevel;

    [Tooltip("Can be null")]
    [SerializeField] private FadeChanger _fadeChanger;

    private int currentPlayers;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent(out PlayerController2d player))
        {
            currentPlayers++;

            if (currentPlayers >= playersNeededToCompleteLevel)
            {
                NextScene();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerController2d player))
        {
            currentPlayers--;
        }
    }

    public void NextScene()
    {
        if (_fadeChanger != null)
        {
            _fadeChanger.StartFadeInAndChangeScene(SceneLoader.GetNextSceneIndex());
        }
        else
        {
            SceneLoader.NextScene();
        }
    }

    public void ToMenu()
    {
        SceneLoader.LoadByIndex(0);
    }

}
