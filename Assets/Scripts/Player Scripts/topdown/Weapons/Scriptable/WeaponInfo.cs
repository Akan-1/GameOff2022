using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class WeaponInfo : ScriptableObject
{
    #region Configuration
    [Space]
    [Header("Weapon Config")]
    [SerializeField] private Sprite _sprite;
    [SerializeField] private Vector2 _divineFirePoint;
    [SerializeField] private int _maximumBulletsInMagazine;
    [SerializeField] private int _bulletPerShot;
    [SerializeField] private int _bulletsAviable;
    [SerializeField] private float _fireRate = 1;
    [SerializeField] private float _scatter;
    [SerializeField] private float _reloadTime;

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
    public float ReloadTime => _reloadTime;
    public int MaximumBulletsInMagazine => _maximumBulletsInMagazine;
    public int BulletPerShot => _bulletPerShot;
    public int BulletsAviable => _bulletsAviable;
    public int CurrentAmmo
    {
        get;
    }
}
