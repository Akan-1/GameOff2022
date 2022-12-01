using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingLamp : Lamp
{

    [SerializeField] private float _rotateSpeed;
    public bool IsRotate
    {
        get;
        set;
    }

    private void Update()
    {
        if (IsRotate)
        {
            transform.Rotate(Vector3.forward * _rotateSpeed);
        }
    }

    public void StartRotate()
    {
        IsRotate = true;
    }
}
