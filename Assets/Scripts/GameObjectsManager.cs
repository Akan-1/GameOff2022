using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectsManager : MonoBehaviour
{
    private void OnEnable()
    {
        GlobalEventManager.onDie += DeleteObjectFromScene;
    }

    private void OnDisable()
    {
        GlobalEventManager.onDie -= DeleteObjectFromScene;
    }

    private void DeleteObjectFromScene(GameObject obj)
    {
        Destroy(obj);
    }

    public static void CheckLifeAmount(int health, GameObject obj)
    {
        if (health <= 0)
        {
            GlobalEventManager.SendDieInfo(obj);
        }
    }

}
