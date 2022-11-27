using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class Lever : MonoBehaviour
{
    private BoxCollider2D _boxCollider;
    private List<PlayerController2d> _playerControllers2D = new List<PlayerController2d>();

    [SerializeField] private AudioSource _clickSound;
    [SerializeField] private UnityEvent _onClick;

    public string CharacterTag
    {
        get;
        private set;
    } = "Tomas";

    public bool IsActive
    {
        get;
        private set;
    } = false;

    private void Start()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        _boxCollider.isTrigger = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (IsActiveCharacterClose())
            {
                CharacterTag = CharacterSwapper.Instance.CurrentPlayerController2D.tag;

                if (!IsActive)
                {
                    IsActive = true;
                }
                else
                {
                    IsActive = false;
                }

                _clickSound.Play();
                _onClick?.Invoke();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerController2d playerController2D))
        {
            _playerControllers2D.Add(playerController2D);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerController2d playerController2D))
        {
            _playerControllers2D.Remove(playerController2D);
        }
    }

    private bool IsActiveCharacterClose()
    {
        return _playerControllers2D.FindIndex(i => i.IsActive == true) > -1;
    }
}
