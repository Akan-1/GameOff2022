using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Enemy, ISoundHearable
{
    private void Update()
    {
        if (IsCanMove)
        {
            MoveToTarget();
        }

        View();
    }

    private void MoveToTarget()
    {
        if (Target != null)
        {
            RotateToTarget();
            Rigidbody2D.velocity = GetMovementVector() * GetCurrentSpeed();

            if (State == States.Patroling)
            {
                Patroling();
            }
            else
            {
                AggressiveToTarget();
            }
        }
        else
        {
            Debug.LogWarning($"Target of {gameObject.name} is null");
        }
    }

    public void HearFrom(Transform target)
    {
        Debug.Log($"hear sound from {target.name}");
    }
}
