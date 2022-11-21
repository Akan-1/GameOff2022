using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeathScreen : MonoBehaviour
{
    public static DeathScreen instance;

    private void OnEnable()
    {
        GameObjectsManager.onShownBar += ShowBar;
    }

    private void OnDisable()
    {
        GameObjectsManager.onShownBar -= ShowBar;
    }

    [SerializeField] private GameObject _panelBar;
    [SerializeField] private TextMeshProUGUI _text;

    [TextArea(3, 10)]
    [SerializeField] private string[] _TomasPhrasesArr;

    [TextArea(3, 10)]
    [SerializeField] private string[] _AlicePhrasesArr;

    private string[] _ChoosePharesArr;

    private string sign;

    private bool _playerIsDead = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }

        Destroy(this);
    }

    private void Update()
    {
        if (_playerIsDead)
        {
            if (Input.GetMouseButtonDown(1))
                UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void ShowBar()
    {
        _text.text = "";
        _playerIsDead = true;
        _panelBar.SetActive(true);
        StartCoroutine(TextCoroutine());
    }

    private string Phares(string tag, string[] pharesArr)
    {
        if(tag == "Tomas")
        {
            pharesArr = _TomasPhrasesArr;
        }
        else if(tag == "Alice")
        {
            pharesArr = _AlicePhrasesArr;
        }
  
        return pharesArr[Random.Range(0, pharesArr.Length)];
    }

    IEnumerator TextCoroutine()
    {
        foreach (char abc in Phares(GameObjectsManager.tagInfo, _ChoosePharesArr))
        {
            _text.text += abc;
            
            sign = abc.ToString();

            if (sign == (".")
             || sign == (",")
             || sign == ("!")
             || sign == ("?"))
            {
                yield return new WaitForSeconds(0.35f); 
            }
            else
            {
                yield return new WaitForSeconds(0.07f); 
            }
        }
    }

}
