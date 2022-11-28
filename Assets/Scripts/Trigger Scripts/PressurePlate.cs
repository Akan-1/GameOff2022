using UnityEngine;
using System;
using UnityEngine.Events;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class PressurePlate : MonoBehaviour
{
    private Animator animator;
    private AudioSource _audioSource;

    [SerializeField] private bool _isNeedHeavyBox;

    [SerializeField] private UnityEvent _onPressed;
    [SerializeField] private UnityEvent _onLeaved;

    [Header("Audio")]
    [SerializeField] private List<AudioClip> _activateSounds = new List<AudioClip>();
    [SerializeField] private List<AudioClip> _disactivateSounds = new List<AudioClip>();
    [SerializeField] private float _volumeScale;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_isNeedHeavyBox)
        {
            bool isCharacter = collision.collider.TryGetComponent(out PlayerController2d playerController2D);
            bool isHeavyBox = collision.collider.TryGetComponent(out HeavyProp heavyProp);

            if (collision.gameObject.CompareTag("Box") || heavyProp || isCharacter)
            {
                animator.SetBool("ButtonPressed", true);
                _onPressed.Invoke();
                AudioPlayer.TryPlayRandom(_audioSource, _activateSounds, _volumeScale);
            }
        }    
        else if (_isNeedHeavyBox)
        {
            if (collision.gameObject.CompareTag("HeavyBox"))
            {
                animator.SetBool("ButtonPressed", true);
                _onPressed.Invoke();
                AudioPlayer.TryPlayRandom(_audioSource, _activateSounds, _volumeScale);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        animator.SetBool("ButtonPressed", false);
                AudioPlayer.TryPlayRandom(_audioSource, _disactivateSounds, _volumeScale);
        _onLeaved?.Invoke();
    }

}
