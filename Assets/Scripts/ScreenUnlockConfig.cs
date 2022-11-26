using UnityEngine;

public class ScreenUnlockConfig : MonoBehaviour
{
    [Space]
    [SerializeField] private Lever _lever;

    [Space]
    [Tooltip("Lever takes true or false statements. If this field is true, it means that lever must be a activated and vice versa.")]
    [SerializeField] private bool _neededLeverPosition;
    
    [Tooltip("This field contain tag of the player, whose must activate lever")]
    [SerializeField] private string _openerTagName;

    [Space]
    [SerializeField] private LocksManager locksManager;

    private bool _isInformationMatch = false;

    public bool InfromationMatch
    {
        get;
        private set;
    }

    public void CheckMatchInfo()
    {
        if(_lever.LeverPosition == _neededLeverPosition && _lever.ObjectTag == _openerTagName)
        {
            _isInformationMatch = true;
            InfromationMatch = _isInformationMatch;
            locksManager.TryUnlockDoor();
        }
    }

}
