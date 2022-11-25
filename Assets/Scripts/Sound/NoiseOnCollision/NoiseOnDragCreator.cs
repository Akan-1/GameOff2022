using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseOnDragCreator : NoiseOnCollisionCreator
{
    [SerializeField] private Rigidbody2D _rigibody2D;

    [SerializeField] private List<AudioClip> _dragSounds = new List<AudioClip>();
    [SerializeField] private AudioSource _audioSource;
    private bool _isDragging;


    private void Update()
    {
        if (Mathf.RoundToInt(_rigibody2D.velocity.x) == 0)
        {
            if (_isDragging)
            {
                StopDragging();
                _isDragging = false;
            }
        }
        else
        {
            if (!_isDragging)
            {
                CurrentPosition = new Vector3(transform.position.x, CurrentPosition.y, CurrentPosition.z);
                NoiseMaker.PlayRandomAudioWithCreateNoiseOnAudioSource(_audioSource, _dragSounds, SoundVolume, NoiseRadius);
                _isDragging = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (_isDragging)
        {
            StopDragging();
            _isDragging = false;
        }
    }

    private void StopDragging()
    {
        Debug.Log("StopDragging");
        StartNoiseDisabler();
        _audioSource.Stop();
    }

}
