using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorsAndPressureController : MonoBehaviour
{
    [SerializeField] private List<Door> _doorList = new List<Door>();
    [SerializeField] private List<PressurePlate> _pressureList = new List<PressurePlate>();
    

    //void TryOpenDoor()
    //{
    //    for (int i = 0; i < _doorList.Count; i++)
    //    {
    //        for (int j = 0; j < _pressureList.Count; j++)
    //        {
    //            if (_pressureList[j].pressureIndex == _doorList[i].doorIndex;
    //        }
    //    }
    //}
}
