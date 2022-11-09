using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public WeaponInfo currentWeapon;
    public WeaponPickUp weaponPick;

    [SerializeField] private float speed;
    private float nextTimeOfFire = 0;

    private Vector2 direction;

    private Rigidbody2D rb;

    private bool haveGun = false;
    
    public Transform firePointPlayer;
    public bool canThrow = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        LookAtMouse();

        if (haveGun)
        {
            if (Input.GetMouseButton(0))
            {
                if (Time.time >= nextTimeOfFire)
                {
                    currentWeapon.Shoot();
                    nextTimeOfFire = Time.time + 1 / currentWeapon.fireRate;
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (canThrow)
                {
                    ThrowAwayWeapon(weaponPick);
                }       
            }

        }
    }

    public bool WeaponInHand(bool inHand)
    {
        haveGun = inHand;
        return true;
    }

    private void FixedUpdate()
    {
        Walk();   
    }

    private void Walk()
    {
        direction.x = Input.GetAxisRaw("Horizontal");
        direction.y = Input.GetAxisRaw("Vertical");

        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
    }

    private void LookAtMouse()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.up = (mousePos - new Vector2(transform.position.x, transform.position.y));
    }

    private void ThrowAwayWeapon(WeaponPickUp weapon)
    {
        canThrow = false;

        //GameObject localWeapon = weapon.gameObject;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 dir;

        dir.x = mousePos.x - weapon.transform.position.x;
        dir.y = mousePos.y - weapon.transform.position.y;
        dir.z = 0;

        currentWeapon = null;

        weapon.gameObject.AddComponent<ThrowingWeapon>();
        weapon.gameObject.GetComponent<ThrowingWeapon>().setDirection(dir);
        weapon.transform.SetParent(null);
        weapon.gameObject.SetActive(true);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out WeaponPickUp weaponPickLocal))
        {
            if (haveGun)
            {
                weaponPick = weaponPickLocal;
                canThrow = true;
            }
        }
    }



    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    if (collision.gameObject.TryGetComponent(out WeaponPickUp weapon))
    //    {
    //        weapon.transform.SetParent(transform);

    //        Rigidbody2D rbw = weapon.GetComponent<Rigidbody2D>();


    //        weapon.gameObject.transform.SetParent(null);
    //        rbw.bodyType = RigidbodyType2D.Dynamic;
    //        Debug.Log("1231");
    //    }
    //}

}
