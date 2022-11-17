using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColdWeapon : MonoBehaviour
{
    [SerializeField] private ColdWeaponInfo _coldWeaponInfo;

    [SerializeField] private float radius;

    private void Start()
    {
        radius = _coldWeaponInfo.radius;
    }

    private void OnDrawGizmos() 
    {
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.color = Color.green;
    }

    private void Update()
    {
        Physics2D.OverlapCircle(transform.position, radius);

    }

}
