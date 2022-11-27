using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleWithScreen : MonoBehaviour
{
    public enum CharactersTags
    {
        Tomas,
        Alice
    }

    [Serializable]
    private class Order
    {
        [SerializeField] private Lever _lever;
        [SerializeField] private ScreenLever _screenLever;
        [SerializeField] private CharactersTags _needCharacter;
        [SerializeField] private bool _isNeedActiveLever;
        public Lever Lever => _lever;
        public ScreenLever ScreenLever => _screenLever;
        public CharactersTags NeedCharacter => _needCharacter;
        public bool IsNeedActiveLever => _isNeedActiveLever;
    }

    [SerializeField] private List<Order> _orders = new List<Order>();
    [Space] [SerializeField] private UnityEvent _onSuccess;


    public void CheckOrder()
    {
        foreach (var order in _orders)
        {
            bool isLeverMatch = order.Lever.IsActive == order.IsNeedActiveLever;
            bool isCharacterMatch = order.Lever.CharacterTag == $"{order.NeedCharacter}";
            bool isConditionNotMatch = !isLeverMatch || !isCharacterMatch;

            if (isConditionNotMatch)
            {
                Debug.Log("ConditionNotMatch");
                return;
            }
        }

        _onSuccess?.Invoke();
    }
}
