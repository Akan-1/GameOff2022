using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController2d))]
public class PlayerObjectMover : MonoBehaviour
{
    [SerializeField] private float _allowableChangePlayerVelocityY;
    [SerializeField] private float _allowableChangeTransformAngularVelocity;
    public PlayerController2d PlayerController2D
    {
        get;
        private set;
    }

    public float AllowableChangePlayerVelocityY => _allowableChangePlayerVelocityY;
    public float AllowableChangeTransformAngularVelocity => _allowableChangeTransformAngularVelocity;



    private void Start()
    {
        PlayerController2D = GetComponent<PlayerController2d>();
    }
}
