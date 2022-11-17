using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public static void TryPlayRandom(AudioSource audioSource, List<AudioClip> audioClips, float volumeScale)
    {
        try
        {
            AudioClip audioClip = audioClips[Random.Range(0, audioClips.Count)];
            audioSource.PlayOneShot(audioClip, volumeScale);
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
            throw;
        }

    }
}
