using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseOnFallCreator : NoiseOnCollisionCreator
{
    [SerializeField] private string _audioSourcePoolName = "AudioSource";
    [SerializeField] private List<AudioClip> _fallSounds = new List<AudioClip>();
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsCanPlayAudio)
        {
            if (Math.Round(CurrentPosition.y, 1) != Math.Round(transform.position.y, 1))
            {
                CurrentPosition = new Vector3(CurrentPosition.x, transform.position.y, CurrentPosition.z);
                NoiseMaker.PlayRandomAudioWithCreateNoise(_fallSounds, SoundVolume, NoiseRadius);
                ReloadAudio();
                StartNoiseDisabler();
            }
        }
    }
}
