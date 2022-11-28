using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public static void TryPlayRandom(AudioSource audioSource, List<AudioClip> audioClips, float volumeScale, float audioSourceRadius = 13)
    {
        try
        {
            if (audioClips.Count > 0)
            {
                AudioClip audioClip = audioClips[Random.Range(0, audioClips.Count)];
                audioSource.clip = audioClip;
                audioSource.volume = volumeScale;
                audioSource.maxDistance = audioSourceRadius;
                audioSource.minDistance = 0;
                audioSource.Play();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
            throw;
        }

    }
}
