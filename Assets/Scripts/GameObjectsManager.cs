using UnityEngine;
using System;

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

    

    public static Action onShownBar;

    public static string tagInfo;

    private void DeleteObjectFromScene(GameObject obj, string tagLocal)
    {
        if (obj.CompareTag("Tomas") || obj.CompareTag("Alice"))
        {
            onShownBar?.Invoke();
            Destroy(obj); //Временное решение, позже, думаю, будем просто спавнить спрайт с анимацией смерти игрока.
        }
        else
            Destroy(obj);
    }

    public static void CheckLifeAmount(int health, GameObject obj, string tag = "")
    {
        if (health <= 0)
        {
            tag = obj.tag;
            tagInfo = tag;
            GlobalEventManager.SendDieInfo(obj, tag);
        }
    }

}
