using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public static void TryPlayRandom(Transform target, AudioSource audioSource, List<AudioClip> audioClips, float volumeScale)
    {
        try
        {
            audioSource.transform.position = target.position;
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
