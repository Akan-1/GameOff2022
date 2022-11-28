using QFSW.MOP2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class NoiseMaker : MonoBehaviour
{
    [SerializeField] private string _audioSourcePoolName = "AudioSource";

    public CircleCollider2D Noise
    {
        get;
        set;
    }

    private void Awake()
    {
        Noise = GetComponent<CircleCollider2D>();
        Noise.isTrigger = true;
        Noise.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out ISoundHearable ISoundHerable))
        {
            ISoundHerable.HearFrom(transform.position);
        }
    }

    public void PlayRandomAudioWithCreateNoise(List<AudioClip> _audioClips, float volumeScale, float noiseRadius, float audioSourceRadius = 13)
    {
        if (MasterObjectPooler.Instance != null)
        {
            AudioSource _audioSource = MasterObjectPooler.Instance.GetObjectComponent<AudioSource>(_audioSourcePoolName);
            _audioSource.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            _audioSource.minDistance = 0;
            _audioSource.maxDistance = audioSourceRadius;

            AudioPlayer.TryPlayRandom(_audioSource, _audioClips, volumeScale);
        } else
        {
            Debug.LogWarning("PoolManager doesn't exist. Audio will not be play. Please add MasterObjectPool script and add pool of audioSource");
        }

        Noise.radius = noiseRadius;
        Noise.enabled = true;
    }

    public void PlayRandomAudioWithCreateNoiseOnAudioSource(AudioSource _audioSource, List<AudioClip> _audioClips, float volumeScale, float noiseRadius, float audioSourceRadius = 13)
    {
        AudioPlayer.TryPlayRandom(_audioSource, _audioClips, volumeScale);
    }

}
