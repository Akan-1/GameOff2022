using UnityEngine;

public class Enemy : MonoBehaviour, ITakeDamage
{
    [SerializeField] private float moveSpeed = 1f;

    [SerializeField] private int _health;
    [SerializeField] private int _damage;


    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _health = Random.Range(2, 5);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsFacingRight())
        {
            rb.velocity = new Vector2(moveSpeed, 0f);
        }
        else
        {
            rb.velocity = new Vector2(-moveSpeed, 0f);
        }
    }

    private bool IsFacingRight()
    {
        return transform.localScale.x > Mathf.Epsilon;
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        GameObjectsManager.CheckLifeAmount(_health, gameObject, gameObject.tag);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        transform.localScale = new Vector2(-(Mathf.Sign(rb.velocity.x)), transform.localScale.y);

        if(collision.gameObject.TryGetComponent(out ITakeDamage takeDamage))
        {
            takeDamage.TakeDamage(_damage);
        }
    }
}
