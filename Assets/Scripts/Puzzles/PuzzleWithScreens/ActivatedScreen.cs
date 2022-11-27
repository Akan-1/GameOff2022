using UnityEngine;

public class ActivatedScreen : MonoBehaviour
{
    [SerializeField] private Lever _lever;

    [Space]
    [Header("Icons")]
    [SerializeField] private Sprite _activatedIconThomas;
    [SerializeField] private Sprite _inactivatedIconThomas;
    [Space]
    [SerializeField] private Sprite _activatedIconAlice;
    [SerializeField] private Sprite _inactivatedIconAlice;

    private SpriteRenderer currentSprite;

    private void Start()
    {
        currentSprite = GetComponent<SpriteRenderer>();
    }

    public void UpdateSprite()
    {
        if (_lever.CharacterTag == "Tomas")
        {
            currentSprite.sprite = _lever.IsActive ? _activatedIconThomas : _inactivatedIconThomas;
        } else
        {
            currentSprite.sprite = _lever.IsActive ? _activatedIconAlice : _inactivatedIconAlice;
        }
    }

}
