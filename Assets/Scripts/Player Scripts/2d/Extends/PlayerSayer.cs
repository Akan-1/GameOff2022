using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerController2d))]
[RequireComponent(typeof(AudioSource))]
public class PlayerSayer : MonoBehaviour
{
    [Serializable]
    public class UnityEventStringList : UnityEvent<List<string>> { }

    private Vector3 _textStarLocalScale;

    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private AnimationCurve _textSclale;
    [SerializeField] [Min(0)] private Vector2 _randomTimeBetweenChar;
    [SerializeField] private List<string> _texts;
    private string _currentText;
    private float _scaleCurrentTime;
    private bool _isSaying;
    private IEnumerator _scaleChange; 
    private IEnumerator _addChar;

    [Header("Events")]
    [SerializeField] [HideInInspector] private UnityEvent _onStartSay;
    [SerializeField] [HideInInspector] private UnityEvent _onEndSay;
    [SerializeField] [HideInInspector] UnityEventStringList _onEndSayTexts;

    [Header("Audio")]
    [SerializeField] private string _audioSourcePoolName = "AudioSource";
    [SerializeField] private float _mumblingVolume = 1;
    [SerializeField] private List<AudioClip> _mumblingSounds = new List<AudioClip>();
    private AudioSource _audioSource;

    public UnityEvent OnStartSay
    {
        get => _onStartSay;
        set => _onStartSay = value;
    }
    public UnityEvent OnEndSay
    {
        get => _onEndSay;
        set => _onEndSay = value;
    }

    public PlayerController2d PlayerController2D
    {
        get;
        private set;
    }

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        PlayerController2D = GetComponent<PlayerController2d>();
        _textStarLocalScale = _text.transform.localScale;
        _text.transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            NextText();
        }
    }

    #region Say
    public void SayFew(List<string> texts)
    {
        _isSaying = true;
        _texts.Clear();
        _texts.AddRange(texts);
        _currentText = _texts[0];

        StartScaleChange(true);
        NextText();

        OnStartSay?.Invoke();
        OnStartSay?.RemoveAllListeners();

        PlayerController2D.LockMovement();
        PlayerController2D.LockShoting();
    }

    private void NextText()
    {
        bool isHasNextText = _texts.Count > 0;
        bool isTextSayed = _text.text == _currentText;

        if (isHasNextText)
        {
            if (isTextSayed)
            {
                _currentText = _texts[0];
                _texts.RemoveAt(0);
            }
        }
        else
        {
            if (isTextSayed)
            {
                PlayerController2D.UnlockMovement();
                PlayerController2D.UnlockShoting();
                StartScaleChange(false);
                OnEndSay?.Invoke();
                OnEndSay?.RemoveAllListeners();
                _isSaying = false;
            }
        }


        PrintText();
    }

    #endregion

    #region CharAdd

    private void PrintText()
    {
        bool isPrinting = _addChar != null;

        if (_currentText != _text.text && _isSaying)
        {
            if (isPrinting)
            {
                StopCoroutine(_addChar);
                _addChar = null;
                _text.text = _currentText;
            } else
            {
                PlayMumblingSound();
                _text.text = "";
                _addChar = AddChar();
                StartCoroutine(_addChar);
            }
        }
    }

    private IEnumerator AddChar()
    {

        foreach (char textChar in _currentText)
        {
            float randomTime = UnityEngine.Random.Range(_randomTimeBetweenChar.x, _randomTimeBetweenChar.y);
            yield return new WaitForSeconds(randomTime);
            _text.text += textChar;
        }

        _addChar = null;
    }

    #endregion

    #region ScaleChange
    private void StartScaleChange(bool isIncrease)
    {
        if (_scaleChange != null)
        {
            StopCoroutine(_scaleChange);
        }

        _scaleChange = ScaleChange(isIncrease);
        StartCoroutine(_scaleChange);
    }

    private IEnumerator ScaleChange(bool isNeedIncrease)
    {
        bool isIncrease = _scaleCurrentTime < _textSclale.keys[_textSclale.length - 1].time;
        bool isDecrease = _scaleCurrentTime > 0;
        bool isChangeScale = isNeedIncrease ? isIncrease : isDecrease;

        while (isChangeScale)
        {
            ChangeScale(isNeedIncrease);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    private void ChangeScale(bool isNeedIncrease)
    {
        _scaleCurrentTime = isNeedIncrease ? _scaleCurrentTime += Time.deltaTime : _scaleCurrentTime -= Time.deltaTime;
        _scaleCurrentTime = Mathf.Clamp(_scaleCurrentTime, 0, _textSclale.keys[_textSclale.length - 1].time);
        float newScale = _textSclale.Evaluate(_scaleCurrentTime);
        _text.transform.localScale = new Vector3(newScale * Mathf.Clamp(PlayerController2D.transform.localScale.x, -1, 1), newScale, newScale);
    }

    #endregion

    #region Audio

    private void PlayMumblingSound()
    {
        AudioPlayer.TryPlayRandom(_audioSource, _mumblingSounds, _mumblingVolume);
    }

    #endregion
}
