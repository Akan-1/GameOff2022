﻿using UnityEngine;

[RequireComponent(typeof(PlayerController2d))]
public class PlayerSquatter : MonoBehaviour
{
    private PlayerController2d _playerController2D;

    [Space]
    [Header("Squat Config")]
    [SerializeField] private LayerMask _roofMask;
    [SerializeField] private Transform _topCheck;
    [SerializeField] private float _topCheckRadius;
    [SerializeField] private Collider2D _poseStand;
    [SerializeField] private Collider2D _poseSquat;
    [SerializeField] private float _squatSpeed = 5;
    private bool _isStand;

    private void Start()
    {
        _playerController2D = GetComponent<PlayerController2d>();
    }

    public bool IsStand
    {
        get => _isStand;
        private set
        {
            Animator _animator = _playerController2D.Animator;

            Debug.Log(value == _isStand);

            if (value != _isStand)
            {
                _playerController2D.PlayIdleAnimation();
                _isStand = value;
            }

            if (value)
            {
                _poseStand.enabled = true;
                _poseSquat.enabled = false;
                _animator.SetBool("IsSquat", false);
                _playerController2D.Speed = _playerController2D.DefaultSpeed;
                _playerController2D.IsCanJump = true;
                _playerController2D.PlayIdleAnimation();
                _animator.SetFloat("Speed", _playerController2D.Speed);
            }
            else
            {
                _poseStand.enabled = false;
                _poseSquat.enabled = true;
                _animator.SetBool("IsSquat", true);
                _playerController2D.Speed = _squatSpeed;
                _playerController2D.IsCanJump = false;
                _animator.SetFloat("Speed", _playerController2D.Speed);
            }
        }
    }

    private void Update()
    {
        if (_playerController2D.IsActive)
        {
            Squat();
        }
    }

    private void Squat()
    {
        bool cantStand = Physics2D.OverlapCircle(_topCheck.position, _topCheckRadius, _roofMask);

        if (!cantStand)
        {
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                IsStand = true;
            }
            else if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                IsStand = false;
            }
        }

        if (cantStand)
        {
            IsStand = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_topCheck.position, _topCheckRadius);
    }

}
