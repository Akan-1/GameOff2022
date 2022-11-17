using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseOnCollisionCreator : MonoBehaviour
{
    [SerializeField] private string _audioSourcePoolName = "AudioSource";
    [SerializeField] private NoiseMaker _noiseMaker;
    [SerializeField] private float _activeNoiseTime = .1f;

    [Header("Settings")]
    [SerializeField] private float _minTimeBetweenAudio;
    private bool _isCanPlayAudio = true;

    [Header("Fall")]
    [SerializeField] private List<AudioClip> FallSounds = new List<AudioClip>();
    [SerializeField] private float _fallNoiseRadius;

    [Header("Drag")]
    [SerializeField] private List<AudioClip> DragSounds = new List<AudioClip>();
    [SerializeField] private float _dragNoiseRadius;
    private Vector3 _currentPosition;

    private IEnumerator _noiseDisabler;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_isCanPlayAudio)
        {
            _noiseMaker.PlayRandomAudioWithCreateNoise(FallSounds, 1, _fallNoiseRadius);
            _isCanPlayAudio = false;
            Invoke(nameof(SetCanPlayAudioTrue), _activeNoiseTime);
            StartNoiseDisabler();
        }
    }

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

    private void SetCanPlayAudioTrue()
    {
        _isCanPlayAudio = true;
    }
}
