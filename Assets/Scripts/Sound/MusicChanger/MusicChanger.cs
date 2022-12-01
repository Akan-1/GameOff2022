using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicChanger : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private List<AudioClip> _music;


    private void Start()
    {
        PlayRandomMusic();
    }

    private void PlayRandomMusic()
    {
        StartCoroutine(Play());
    }

    private IEnumerator Play()
    {
        int randomMusicIndex = Random.Range(0, _music.Count - 1);
        AudioClip _randomMusic = _music[randomMusicIndex];

        _audioSource.clip = _randomMusic;
        _audioSource.Play();
        yield return new WaitForSeconds(_audioSource.clip.length);

        PlayRandomMusic();
    }
}
