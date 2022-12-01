using UnityEngine;
using System;

public class GlobalEventManager : MonoBehaviour
{
    public static Action<GameObject, string> onDie;

    public static void SendDieInfo(GameObject obj, string tag)
    {
        onDie?.Invoke(obj, tag);
    }
    

}
