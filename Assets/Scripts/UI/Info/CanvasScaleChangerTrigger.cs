using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasScaleChangerTrigger : MonoBehaviour
{
    [SerializeField] private AnimationCurve _scaleByTime;
    [SerializeField] private Canvas _canvas;
    private float _currentTime;
    private IEnumerator _scaleChanger;

    private void Start()
    {
        _canvas.transform.localScale = Vector3.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerController2d playerController2D) && gameObject.activeInHierarchy)
        {
            StartScaleIncrease();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerController2d playerController2D) && gameObject.activeInHierarchy)
        {
            StartScaleDecrease();
        }
    }

    private void StartScaleIncrease()
    {
        StopScaleChange();

        _scaleChanger = ScaleIncrease();
        StartCoroutine(_scaleChanger);
    }

    private void StartScaleDecrease()
    {
        StopScaleChange();

        _scaleChanger = ScaleDecrease();
        StartCoroutine(_scaleChanger);
    }

    private void StopScaleChange()
    {
        if (_scaleChanger != null)
        {
            StopCoroutine(_scaleChanger);
        }
    }

    private IEnumerator ScaleIncrease()
    {
        float toTime = _scaleByTime.keys[_scaleByTime.length - 1].time;

        while (_currentTime < toTime)
        {
            _currentTime += Time.deltaTime;

            yield return new WaitForSeconds(Time.deltaTime);
            _canvas.transform.localScale = new Vector3(_scaleByTime.Evaluate(_currentTime), _scaleByTime.Evaluate(_currentTime), _scaleByTime.Evaluate(_currentTime)); ;
        }

    }

    private IEnumerator ScaleDecrease()
    {
        float toTime = 0;

        while (_currentTime > toTime)
        {
            _currentTime -= Time.deltaTime;

            yield return new WaitForSeconds(Time.deltaTime);
            _canvas.transform.localScale = new Vector3(_scaleByTime.Evaluate(_currentTime), _scaleByTime.Evaluate(_currentTime), _scaleByTime.Evaluate(_currentTime)); ;
        }
    }

}
