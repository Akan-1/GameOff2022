using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.Events;
using System.Collections.Generic;

public class PrintEffect : MonoBehaviour
{
    [SerializeField] private bool _isPrintOnStart;

    [SerializeField] private TextMeshProUGUI _tmpro;
    [TextArea(4, 10)]
    [SerializeField] private string _text;
    [SerializeField] private float _timeBetweenChar;
    [SerializeField] private float _timeBetweenPunctuationMarks;
    [SerializeField] private UnityEvent _onEndPrint;

    [Space]
    [SerializeField] private List<char> _punctuationMarks = new List<char>();

    private void Awake()
    {
        _tmpro.text = "";
    }

    private void Start()
    {

        if (_isPrintOnStart)
        {
            StartPrint();
        }
    }
    public void StartPrint()
    {
        StartCoroutine(Print());
    }

    private IEnumerator Print()
    {
        foreach (char item in _text)
        {

            yield return new WaitForSeconds(_punctuationMarks.Exists(x => x == item) ? _timeBetweenPunctuationMarks : _timeBetweenChar);
            _tmpro.text += item;
        }
        _onEndPrint?.Invoke();
    }
}
