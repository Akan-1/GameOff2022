using System.Collections;
using UnityEngine;

public class RandomAmbientSounds : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] audioClipsArr;

    [SerializeField] private float timer;

    // Start is called before the first frame update
    //void Start()
    //{
    //    timeLeft = timer;
    //}

    IEnumerator Ambient()
    {
        yield return new WaitForSeconds(timer);
        audioSource.PlayOneShot(audioClipsArr[Random.Range(0, audioClipsArr.Length)], 0.5f);
    }

    void Update()
    {
        StartCoroutine(Ambient());
    }
}
