using UnityEngine;

public class ThrowingWeapon : MonoBehaviour
{
    float timer = 2.0f;
    Vector3 direction;
    Rigidbody2D rid;
    GameObject player;
    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rid = this.GetComponent<Rigidbody2D>();
        rid.AddForce(direction * 40);
    }

    // Update is called once per frane
    void Update()
    {
        transform.rotation = Quaternion.Slerp(this.transform.rotation, new Quaternion(this.transform.rotation.x, this.transform.rotation.y, this.transform.rotation.z - 1, this.transform.rotation.w), Time.deltaTime * timer);
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            rid.isKinematic = true;
            Destroy(this);
        }
    }

    public void setDirection(Vector3 dir)
    {
        direction = dir;
    }
}
