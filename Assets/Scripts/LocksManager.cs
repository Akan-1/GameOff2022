using System.Collections.Generic;
using UnityEngine;

public class LocksManager : MonoBehaviour
{
    [SerializeField] private List<ScreenUnlockConfig> _configList = new List<ScreenUnlockConfig>();
    [Space]
    [SerializeField] private Door _door;

    public void TryUnlockDoor()
    {
        for (int i = 0; i <= _configList.Count; i++)
        {
            if(_configList[i].InfromationMatch)
            {
                _door.OpenDoor();
                
            }
        }
    }
}