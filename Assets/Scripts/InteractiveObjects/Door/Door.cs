using UnityEngine;
public class Door : MonoBehaviour
{
    BoxCollider2D boxCollider;
    Animator animator;
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    public void OpenDoor()
    {
        animator.SetBool("DoorIsOpen", true);
    }

    public void CloseDoor()
    {
        animator.SetBool("DoorIsOpen", false);
    }
}
