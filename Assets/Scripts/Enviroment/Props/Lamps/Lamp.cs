using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Lamp : MonoBehaviour
{
    private float _currentTime;
    [SerializeField] private Light2D _lightSource;
    [SerializeField] private AnimationCurve _lightIntencity;
    [SerializeField] private Vector2 _startChangeIntencityTimeBetween;

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] [Range(0, 1f)] private float _volumeScaler = 1;

    private void Start()
    {
        float time = Random.Range(_startChangeIntencityTimeBetween.x, _startChangeIntencityTimeBetween.y);
        _audioSource.volume = _lightIntencity.Evaluate(_currentTime) * _volumeScaler;
        Invoke(nameof(StartChangeIntencity), time);
    }

    private void StartChangeIntencity()
    {
        StartCoroutine(Light());
    }

    private IEnumerator Light()
    {
        while (true)
        {
            _lightSource.intensity = _lightIntencity.Evaluate(_currentTime);
            _audioSource.volume = _lightIntencity.Evaluate(_currentTime) * _volumeScaler;

            yield return new WaitForSeconds(Time.deltaTime);

            if (_currentTime >= _lightIntencity.keys[_lightIntencity.length - 1].time)
            {
                _currentTime = 0;
            } else
            {
                _currentTime += Time.deltaTime;
            }

        }
    }
}
