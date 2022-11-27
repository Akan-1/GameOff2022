using UnityEngine;
using System;
using UnityEngine.Events;

public class Lever : MonoBehaviour
{

    [SerializeField] private AudioSource _clickSound;

    [SerializeField] private UnityEvent _onClick;

    public string CharacterTag
    {
        get;
        private set;
    }

    public bool IsActive
    {
        get;
        private set;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerController2d player))
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (!IsActive)
                {
                    IsActive = true;
                }
                else
                {
                    IsActive = false;
                }

                CharacterTag = player.tag;
                _clickSound.Play();
                _onClick?.Invoke();
            }

            Debug.Log(player.name);
        }
    }
}
