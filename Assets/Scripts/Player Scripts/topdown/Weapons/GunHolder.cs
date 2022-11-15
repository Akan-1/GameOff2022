using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GunHolder : MonoBehaviour
{
    private PlayerController2d _playerController2D;
    private SpriteRenderer _spriteRenderer;

    [SerializeField] private float _throwOutAngularVelocity = 245;
    [SerializeField] private float _throwForce = 3;
    public Weapon Weapon
    {
        get;
        set;
    }
    public bool IsCanThrowWeapon
    {
        get;
        set;
    }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerController2D = transform.parent.GetComponent<PlayerController2d>();
    }

    private void Update()
    {
        if (Weapon != null)
        {
            if (Input.GetMouseButtonUp(1))
            {
                IsCanThrowWeapon = true;
            }

            if (Input.GetMouseButton(0))
            {
                Weapon.ShotFrom(_playerController2D);
            }
            else if (Input.GetMouseButtonDown(1) && IsCanThrowWeapon)
            {
                ThrowOut();
            }
        }
    }

    public void LoadWeapon(Weapon weapon)
    {
        _spriteRenderer.sprite = weapon.WeaponInfo.Sprite;
        weapon.gameObject.SetActive(false);
        Weapon = weapon;
        Weapon.Reload();
    }

    public void ClearWeapon()
    {
        _spriteRenderer.sprite = null;
        Weapon = null;
    }

    private void ThrowOut()
    {

        Weapon.transform.position = transform.position;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 dir;

        dir.x = mousePos.x - Weapon.transform.position.x;
        dir.y = mousePos.y - Weapon.transform.position.y;
        dir.z = 0;

        Weapon.transform.SetParent(null);
        Weapon.gameObject.SetActive(true);

        Weapon.Rigibody2D.AddForce(dir * _throwForce, ForceMode2D.Impulse);
        Weapon.Rigibody2D.angularVelocity = _throwOutAngularVelocity;

        ClearWeapon();

    }

    
}
