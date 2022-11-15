using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class WeaponInfo : ScriptableObject
{
    #region Configuration
    [Space]
    [Header("Weapon Config")]
    [SerializeField] private Sprite _sprite;
    [SerializeField] private Vector2 _divineFirePoint;
    [SerializeField] private int _maximumAmmo;
    [SerializeField] private float _fireRate = 1;
    [SerializeField] private float _scatter;

    [Space]
    [Header("Bullet Config")]
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Vector2 _bulletForceBetween;
    [SerializeField] private float _secondsBeforeNextShot;

    #endregion

    public Sprite Sprite => _sprite;
    public Vector2 DivineFirePoint => _divineFirePoint;
    public float FireRate => _fireRate;
    public float Scatter => _scatter;
    public GameObject BulletPrefab => _bulletPrefab;
    public Vector2 BulletForceBetween => _bulletForceBetween;
    public float SecondsBeforeNextShot => _secondsBeforeNextShot;
    public int MaximumAmmo => _maximumAmmo;
    public int CurrentAmmo
    {
        get;
    }
}
