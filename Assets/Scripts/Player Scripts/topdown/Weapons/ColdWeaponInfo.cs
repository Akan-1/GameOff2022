using UnityEngine;

[CreateAssetMenu(fileName = "New Cold Weapon", menuName = "Cold Weapon")]
public class ColdWeaponInfo : ScriptableObject
{
    [SerializeField] private Sprite _spriteWeapon;

    [SerializeField] private Animator _attackAnimation;

    [SerializeField] private float _radius;
    [SerializeField] private float _cooldown;
    [SerializeField] private float _damage;

    public float radius => _radius;
    public float cooldown => _cooldown;
    public float damage => _damage;

}
