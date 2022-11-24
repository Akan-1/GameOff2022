using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CharacterSwapper : MonoBehaviour
{
    public static CharacterSwapper Instance;
    private PlayerController2d _character;
    private CameraFollow _cameraFollow;

    [SerializeField] AudioSource _audioSource;
    [SerializeField] private AudioClip _swapSound;
    
    [Space]
    private List<PlayerController2d> _possibleCharacters = new List<PlayerController2d>();
    [SerializeField] private int _whichCharacter;

    public PlayerController2d CurrentPlayerController2D
    {
        get => _character;
        set
        {
            _character = value;
            _cameraFollow.StartFollowTo(CurrentPlayerController2D.transform);
        }
    }

    private void Awake()
    {
        _cameraFollow = Camera.main.GetComponent<CameraFollow>();
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }

        Destroy(this);
    }

    void Start()
    {

        if (_possibleCharacters.Count > 0)
        {
            ActiveCharacter(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwapToPreviousCharacter();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            SwapToNextCharacter();

        }
    }

    #region Swap

    private void SwapToNextCharacter()
    {
        int indexOfNextCharacter = _whichCharacter += 1;
        _whichCharacter = _whichCharacter < _possibleCharacters.Count ? indexOfNextCharacter : 0;
        Swap();
    }

    private void SwapToPreviousCharacter()
    {
        int indexOfPreviousCharacter = _whichCharacter -= 1;
        int indexOfLastCharacter = _possibleCharacters.Count - 1;
        _whichCharacter = _whichCharacter > -1 ? indexOfPreviousCharacter : indexOfLastCharacter;
        Swap();
    }

    private void Swap()
    {
        if (_possibleCharacters.Count > 0)
        {
            ActiveCharacter(_whichCharacter);
            DisactiveNonCurrentCharacters();
            _audioSource.PlayOneShot(_swapSound);
        }
    }

    private void ActiveCharacter(int _characterIndex)
    {
        CurrentPlayerController2D = _possibleCharacters[_characterIndex];
        CurrentPlayerController2D.enabled = true;
        CurrentPlayerController2D.IsActive = true;
    }

    private void DisactiveNonCurrentCharacters()
    {
        for (int i = 0; i < _possibleCharacters.Count; i++)
        {
            if (_possibleCharacters[i] != CurrentPlayerController2D)
            {
                _possibleCharacters[i].StopWalkAninmation();
                _possibleCharacters[i].Rigibody2D.velocity = Vector2.zero;
                _possibleCharacters[i].enabled = false;
                _possibleCharacters[i].IsActive = false;
            }
        }
    }

    #endregion

    #region Clear/Add

    public void ClearCharacters()
    {
        _possibleCharacters = new List<PlayerController2d>();
    }

    public void AddCharacter(PlayerController2d playerController2D)
    {
        _possibleCharacters.Add(playerController2D);

        bool _isCurrentCharacterIsNull = CurrentPlayerController2D == null;
        bool _areHavePossibleCharacters = _possibleCharacters.Count > 0;

        if (_isCurrentCharacterIsNull && _areHavePossibleCharacters)
        {
            ActiveCharacter(_whichCharacter);
        }
    }

    #endregion
}
