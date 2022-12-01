
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;

    private Vector2 direction;

    private Rigidbody2D rb;
    
    public Transform firePointPlayer;
    public bool canThrow = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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

}
