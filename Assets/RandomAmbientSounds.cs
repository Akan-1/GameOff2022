using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAmbientSounds : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private List<AudioClip> _ambientSounds = new List<AudioClip>();
    [SerializeField] private float volumeScater = 0.5f;

    [SerializeField] private float timer;

    public void Start()
    {
        StartCoroutine(Play());
    }

    IEnumerator Play()
    {
        while (true)
        {
            yield return new WaitForSeconds(timer);
            AudioPlayer.TryPlayRandom(transform, _audioSource, _ambientSounds, volumeScater);
        }
    }
}
