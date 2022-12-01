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
        boxCollider.isTrigger = true;
        animator.SetBool("DoorIsOpen", true);
    }

    public void CloseDoor()
    {
        boxCollider.isTrigger = false;
        animator.SetBool("DoorIsOpen", false);
    }
}
