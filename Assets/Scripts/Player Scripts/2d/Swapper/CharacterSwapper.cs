using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CharacterSwapper : MonoBehaviour
{
    public static CharacterSwapper Instance;
    private PlayerController2d _character;
    private CameraBehaviour _cameraFollow;

    [SerializeField] AudioSource _audioSource;
    [SerializeField] private AudioClip _swapSound;
    
    [Space]
    private List<PlayerController2d> _possibleCharacters = new List<PlayerController2d>();

    public int CurrentCharacterIndex
    {
        get;
        set;
    } = 0;

    public PlayerController2d CurrentPlayerController2D
    {
        get => _character;
        set
        {
            _character = value;
        }
    }

    public bool IsLockSwap
    {
        get;
        set;
    } = false;

    private void Awake()
    {
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
            ActiveCharacter();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsLockSwap)
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
    }

    #region Swap

    private void SwapToNextCharacter()
    {
        int indexOfNextCharacter = CurrentCharacterIndex + 1;
        CurrentCharacterIndex = CurrentCharacterIndex < _possibleCharacters.Count - 1 ? indexOfNextCharacter : 0;
        Swap();
    }

    private void SwapToPreviousCharacter()
    {
        int indexOfPreviousCharacter = CurrentCharacterIndex - 1;
        int indexOfLastCharacter = _possibleCharacters.Count - 1;
        CurrentCharacterIndex = CurrentCharacterIndex > 0 ? indexOfPreviousCharacter : indexOfLastCharacter;
        Swap();
    }

    private void Swap()
    {
        if (_possibleCharacters.Count > 0)
        {
            ActiveCharacter();
            DisactiveNonCurrentCharacters();
            _audioSource.PlayOneShot(_swapSound);
        }
    }

    private void ActiveCharacter()
    {
        if (_possibleCharacters.Count > 0)
        {
            CurrentPlayerController2D = _possibleCharacters[CurrentCharacterIndex];
            CurrentPlayerController2D.enabled = true;
            CurrentPlayerController2D.IsActive = true;

            _cameraFollow.Target = CurrentPlayerController2D.transform;
        }
    }

    private void DisactiveNonCurrentCharacters()
    {
        for (int i = 0; i < _possibleCharacters.Count; i++)
        {
            if (_possibleCharacters[i] != CurrentPlayerController2D)
            {
                _possibleCharacters[i].StopAninmationBool();
                _possibleCharacters[i].PlayIdleAnimation();
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
        _possibleCharacters.Clear();
    }

    public void AddCharacter(PlayerController2d playerController2D)
    {
        _cameraFollow = Camera.main.GetComponent<CameraBehaviour>();

        _possibleCharacters.Add(playerController2D);

        bool _isCurrentCharacterIsNull = CurrentPlayerController2D == null;
        bool _areHavePossibleCharacters = _possibleCharacters.Count > 0;

        if (_isCurrentCharacterIsNull && _areHavePossibleCharacters)
        {
            CurrentCharacterIndex = 0;
            ActiveCharacter();
        }
    }

    #endregion
}
