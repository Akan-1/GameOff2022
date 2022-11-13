using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GlobalEventManager : MonoBehaviour
{
    public static Action<GameObject> onDie;

    public static void SendDieInfo(GameObject obj)
    {
        onDie?.Invoke(obj);
    }
    

}
