using UnityEngine;

public class ScreenLever : MonoBehaviour
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
        switch (_lever.CharacterTag)
        {
            case "Tomas":

                if (_lever.IsActive)
                    currentSprite.sprite = _activatedIconThomas;
                else
                    currentSprite.sprite = _inactivatedIconThomas;
                break;

            case "Alice":

                if (_lever.IsActive)
                    currentSprite.sprite = _activatedIconAlice;
                else
                    currentSprite.sprite = _inactivatedIconAlice;
                break;
            default:
                Debug.Log("Chracter tag not found");
                break;
        }
    }

}
