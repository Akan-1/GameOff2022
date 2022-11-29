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

    [SerializeField] private string _aliceTag = "Alice";
    [SerializeField] private bool _isAliceStartDialog;
    [SerializeField] private List<Dialog> _dialog = new List<Dialog>();

    public int DialogIndex
    {
        get => _dialogIndex;
        private set
        {
            _dialogIndex = value;

            Debug.Log($"Dialog Index: {value}, Dialogs count: {_dialog.Count}");

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
        _alicePlayerSayer.PlayerController2D.FlipTo((int)_thomasPlayerSayer.transform.position.x > _alicePlayerSayer.transform.position.x ? 1: -1);
        _thomasPlayerSayer.PlayerController2D.FlipTo((int)_thomasPlayerSayer.transform.position.x < _alicePlayerSayer.transform.position.x ? 1 : -1);

        _alicePlayerSayer.PlayerController2D.LockMovement();
        _thomasPlayerSayer.PlayerController2D.LockMovement();

        CharacterSwapper.Instance.IsLockSwap = true;

        NextDialog();
    }

    private void NextDialog()
    {
        _alicePlayerSayer.OnEndSay.RemoveAllListeners();
        _thomasPlayerSayer.OnEndSay.RemoveAllListeners();

        _alicePlayerSayer.PlayerController2D.LockMovement();
        _thomasPlayerSayer.PlayerController2D.LockMovement();

        _alicePlayerSayer.OnEndSay?.AddListener(() => ChangeSayPlayerBool());
        _thomasPlayerSayer.OnEndSay?.AddListener(() => ChangeSayPlayerBool());

        PlayerSayer _currentPlayerSayer = _isAliceStartDialog ? _alicePlayerSayer : _thomasPlayerSayer;
        PlayerSayer _nextPlayer = _isAliceStartDialog ? _thomasPlayerSayer : _alicePlayerSayer;
        List<string> _currentDialog = _thomasPlayerSayer ? _dialog[DialogIndex].FirstCharacterTexts : _dialog[DialogIndex].SecondCharacterTexts;
        List<string> _nextDialog = _thomasPlayerSayer ? _dialog[DialogIndex].SecondCharacterTexts : _dialog[DialogIndex].FirstCharacterTexts;

        _currentPlayerSayer.SayFew(_currentDialog);
        _nextPlayer.OnEndSay?.AddListener(() => DialogIndex++);
        _currentPlayerSayer.OnEndSayTexts.AddListener(AddTextToNextCharacter);
        _currentPlayerSayer.OnEndSay?.AddListener(() => _nextPlayer.SayNextTextOfDialog(_nextDialog));

    }

    private void EndDialog()
    {

        _alicePlayerSayer.OnEndSay.RemoveAllListeners();
        _thomasPlayerSayer.OnEndSay.RemoveAllListeners();

        _alicePlayerSayer.PlayerController2D.UnlockMovement();
        _thomasPlayerSayer.PlayerController2D.UnlockMovement();

        CharacterSwapper.Instance.IsLockSwap = false;
        gameObject.SetActive(false);
    }

    #endregion

    #region Events

    public void AddTextToNextCharacter(List<string> nextText)
    {

        nextText = !_isAliceSay ? _dialog[DialogIndex].SecondCharacterTexts : _dialog[DialogIndex].FirstCharacterTexts;

        if (_isAliceSay)
        {
            _thomasPlayerSayer.SayFew(nextText);
        }
        else
        {
            _alicePlayerSayer.SayFew(nextText);
        }

    }

    #endregion

    public void ChangeSayPlayerBool()
    {
        Debug.Log($"Is Alice Say: {_isAliceSay}");
        _isAliceSay = !_isAliceSay;
    }

}
