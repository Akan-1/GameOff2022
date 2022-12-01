using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseOnCollisionCreator : MonoBehaviour
{
    [SerializeField] private NoiseMaker _noiseMaker;
    [SerializeField] private float _activeNoiseTime = .1f;
    [SerializeField] private float _soundVolume = .5f;
    [SerializeField] private float _noiseRadius;
    [SerializeField] private float _audioSourceRadius = 13;
    private Vector3 _currentPosition;

    [Header("Settings")]
    [SerializeField] private float _minTimeBetweenAudio;
    public NoiseMaker NoiseMaker
    {
        get => _noiseMaker;
        set => _noiseMaker = value;
    }
    public Vector3 CurrentPosition
    {
        get => _currentPosition;
        set => _currentPosition = value;
    }
    public float SoundVolume => _soundVolume;
    public float NoiseRadius => _noiseRadius;
    public float ActiveNoiseTime => _activeNoiseTime;
    public float AudioSourceRadius => _audioSourceRadius;

    public bool IsCanPlayAudio
    {
        get;
        set;
    } = true;

    private IEnumerator _noiseDisabler;

    #region Noise
    public void StartNoiseDisabler()
    {
        StopNoiseDisabler();

        _noiseDisabler = NoiseDisabler();
        StartCoroutine(_noiseDisabler);
    }

    private void StopNoiseDisabler()
    {
        if (_noiseDisabler != null)
        {
            StopCoroutine(_noiseDisabler);
        }
    }

    private IEnumerator NoiseDisabler()
    {
        yield return new WaitForSeconds(_activeNoiseTime);
        _noiseMaker.Noise.enabled = false;
    }

    #endregion

    #region ReloadAudio
    public void ReloadAudio()
    {
        IsCanPlayAudio = false;
        Invoke(nameof(SetCanPlayAudioTrue), _activeNoiseTime);
    }

    private void SetCanPlayAudioTrue()
    {
        IsCanPlayAudio = true;
    }
    #endregion
}
