using UnityEngine;

public class ScreenPuzzle : MonoBehaviour
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

    private void OnEnable()
    {
        _lever.onResultingTag += ChooseNeededSprite;
    }
    private void OnDisable()
    {
        _lever.onResultingTag -= ChooseNeededSprite;
    }

    private void Start()
    {
        currentSprite = GetComponent<SpriteRenderer>();
    }

    private void ChooseNeededSprite(string tag)
    {
        tag = _lever.ObjectTag;

        switch (tag)
        {
            case "Tomas":
                
                if (_lever.LeverPosition)
                    currentSprite.sprite = _activatedIconThomas;
                else
                    currentSprite.sprite = _inactivatedIconThomas;
                break;

            case "Alice":
                
                if (_lever.LeverPosition)
                    currentSprite.sprite = _activatedIconAlice;
                else if(!_lever.LeverPosition) 
                    currentSprite.sprite = _inactivatedIconAlice;
                break;
        }
    }

}
