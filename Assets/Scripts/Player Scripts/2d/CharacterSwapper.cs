using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CharacterSwapper : MonoBehaviour
{
    public static CharacterSwapper Instance;
    private PlayerController2d character;
    private bool _isLoadingSwap = true;

    [SerializeField] AudioSource _audioSource;
    [SerializeField] private AudioClip _swapSound;
    
    [Space]
    private List<PlayerController2d> possibleCharacters = new List<PlayerController2d>();
    [SerializeField] private int whichCharacter;

    // Start is called before the first frame update

    private void Awake()
    {
        //if (Instance == null)
        //{
        //    Instance = this;
        //    DontDestroyOnLoad(gameObject);
        //    return;
        //}

        Destroy(this);
    }

    void Start()
    {
        if (character == null && possibleCharacters.Count > 0)
        {
            character = possibleCharacters[0]; 
        }
        Swap();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (whichCharacter == 0)
            {
                whichCharacter = possibleCharacters.Count - 1;
            }
            else
            {
                whichCharacter -= 1;
            }
            Swap();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (whichCharacter == possibleCharacters.Count - 1)
            {
                whichCharacter = 0;
            }
            else
            {
                whichCharacter += 1;
            }
            Swap();
        }
    }

    public void ClearCharacters()
    {
        possibleCharacters = new List<PlayerController2d>();
    }

    public void AddCharacter(PlayerController2d playerController2D)
    {
        possibleCharacters.Add(playerController2D);

        if (character == null)
        {
            Swap();
        }
    }

    private void Swap()
    {
        if (possibleCharacters.Count > 0)
        {
            character = possibleCharacters[whichCharacter];
            character.enabled = true;
            character.IsActive = true;

            for (int i = 0; i < possibleCharacters.Count; i++)
            {
                if (possibleCharacters[i] != character)
                {
                    if (possibleCharacters[i].Rigibody2D != null)
                    {
                        possibleCharacters[i].Rigibody2D.velocity = Vector2.zero;
                    }

                    possibleCharacters[i].enabled = false;
                    possibleCharacters[i].IsActive = false;
                }
            }

            if (_isLoadingSwap)
            {
                _isLoadingSwap = false;
            }
            else
            {
                _audioSource.PlayOneShot(_swapSound);
            }

        }
    }
}
