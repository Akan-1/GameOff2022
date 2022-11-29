using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class DialogTrigger : MonoBehaviour
{
    [Serializable]
    private class Dialog
    {
        [SerializeField] private List<string> _firstCharacterTexts = new List<string>();
        [SerializeField] private List<string> _secondCharacterTexts = new List<string>();
        public List<string> FirstCharacterTexts => _firstCharacterTexts;
        public List<string> SecondCharacterTexts => _secondCharacterTexts;
    }

    private BoxCollider2D _boxCollider;

    private PlayerSayer _thomasPlayerSayer;
    private PlayerSayer _alicePlayerSayer;

    private int _dialogIndex = 0;
    private bool _isAliceSay;
    private bool _isDialogueStart;

    [SerializeField] private string _aliceTag = "Alice";
    [SerializeField] private bool _isAliceStartDialog;
    [SerializeField] private List<Dialog> _dialog = new List<Dialog>();

    public int DialogIndex
    {
        get => _dialogIndex;
        private set
        {
            _dialogIndex = value;


            if (value < _dialog.Count)
            {
                NextDialog();
                return;
            }

            EndDialog();
        }
    }


    private void Start()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerSayer _playerSayer))
        {
            if (_playerSayer.CompareTag(_aliceTag))
            {
                _alicePlayerSayer = _playerSayer;
            } else
            {
                _thomasPlayerSayer = _playerSayer;
            }
        }

        bool _allPlayerInTrigger = _thomasPlayerSayer != null && _alicePlayerSayer != null;

        if (_allPlayerInTrigger)
        {
            StartDialog();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerSayer _playerSayer))
        {
            if (_playerSayer.CompareTag(_aliceTag))
            {
                _alicePlayerSayer = null;
            }
            else
            {
                _thomasPlayerSayer = null;
            }
        }
    }

    #region Dialog

    private void StartDialog()
    {
        if (!_isDialogueStart)
        {

            DialogIndex = 0;

            _alicePlayerSayer.OnEndSay.RemoveAllListeners();
            _thomasPlayerSayer.OnEndSay.RemoveAllListeners();

            _alicePlayerSayer.PlayerController2D.FlipTo((int)_thomasPlayerSayer.transform.position.x > _alicePlayerSayer.transform.position.x ? 1 : -1);
            _thomasPlayerSayer.PlayerController2D.FlipTo((int)_thomasPlayerSayer.transform.position.x < _alicePlayerSayer.transform.position.x ? 1 : -1);

            _alicePlayerSayer.PlayerController2D.LockMovement();
            _thomasPlayerSayer.PlayerController2D.LockMovement();

            CharacterSwapper.Instance.IsLockSwap = true;

            _isDialogueStart = true;

            NextDialog();

        }
    }

    private void NextDialog()
    {
        _isAliceSay = _isAliceStartDialog;

        PlayerSayer _currentPlayerSayer = _isAliceStartDialog ? _alicePlayerSayer : _thomasPlayerSayer;
        PlayerSayer _nextPlayer = _isAliceStartDialog ? _thomasPlayerSayer : _alicePlayerSayer;
        List<string> _currentDialog = _thomasPlayerSayer ? _dialog[DialogIndex].FirstCharacterTexts : _dialog[DialogIndex].SecondCharacterTexts;
        List<string> _nextDialog = _thomasPlayerSayer ? _dialog[DialogIndex].SecondCharacterTexts : _dialog[DialogIndex].FirstCharacterTexts;

        _currentPlayerSayer.PlayerController2D.LockMovement();
        _currentPlayerSayer.OnEndSay?.AddListener(() => ChangeSayPlayerBool());
        _currentPlayerSayer.OnEndSay?.AddListener(() => _nextPlayer.SayFew(_nextDialog));
        _currentPlayerSayer.OnEndSay?.AddListener(() => _nextPlayer.OnEndSay?.AddListener(() => AddDialogIndex()));

        _nextPlayer.PlayerController2D.LockMovement();
        _nextPlayer.OnEndSay?.AddListener(() => ChangeSayPlayerBool());

        _currentPlayerSayer.SayFew(_currentDialog);

    }

    private void EndDialog()
    {
        Debug.Log("End Dialog");

        _isDialogueStart = false;

        _alicePlayerSayer.OnEndSay.RemoveAllListeners();
        _thomasPlayerSayer.OnEndSay.RemoveAllListeners();

        _alicePlayerSayer.PlayerController2D.UnlockMovement();
        _thomasPlayerSayer.PlayerController2D.UnlockMovement();

        CharacterSwapper.Instance.IsLockSwap = false;

        gameObject.SetActive(false);
    }

    #endregion

    private void AddDialogIndex()
    {
        DialogIndex += 1;
        Debug.Log($"Dialog Index: {DialogIndex}, Dialogs count: {_dialog.Count}");
    }
    public void ChangeSayPlayerBool()
    {
        _isAliceSay = !_isAliceSay;
        Debug.Log($"Is Alice Say: {_isAliceSay}");
    }

}
