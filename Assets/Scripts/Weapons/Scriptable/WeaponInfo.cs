using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class WeaponInfo : ScriptableObject
{
    #region Configuration
    [Space]
    [Header("Weapon Config")]
    [SerializeField] private Sprite _sprite;
    [SerializeField] private List<AudioClip> _pickUpSounds = new List<AudioClip>();
    [SerializeField] private WeaponTypes _type;
    [SerializeField] private ObjectPoolsNames _bulletPoolName;
    [SerializeField] private Vector2 _FirePoint;
    [SerializeField] [Min(1)] private int _maximumBulletsInMagazine;
    [SerializeField] [Min(1)] private int _bulletPerShot;
    [SerializeField] [Min(1)] private int _bulletsAviable;
    [SerializeField] [Min(1)] private float _fireRate = 1;
    [SerializeField] [Min(0)] private float _scatter;
    [SerializeField] [Min(0)] private float _shotNoiseRadius;
    [SerializeField] [Min(0)] private float _recoilStrength;
    [SerializeField] [Min(0)] private float _throwForce = 2;
    [SerializeField] [Min(0.01f)] private float _reloadTime;
    [SerializeField] [Min(.1f)] private float _secondsBeforeNextShot;
    [SerializeField] private bool _isLockJump;

    [Space]
    [Header("Bullet Config")]
    [SerializeField] private Vector2 _bulletForceBetween;

    #endregion

    public Sprite Sprite => _sprite;
    public List<AudioClip> PickUpSounds => _pickUpSounds;
    public ObjectPoolsNames BulletPoolName => _bulletPoolName;
    public WeaponTypes Type => _type;
    public Vector2 OffsetFirePoint => _FirePoint;
    public float FireRate => _fireRate;
    public float Scatter => _scatter;
    public Vector2 BulletForceBetween => _bulletForceBetween;
    public float SecondsBeforeNextShot => _secondsBeforeNextShot;
    public float ReloadTime => _reloadTime;
    public float ShotNoiseRadius => _shotNoiseRadius;
    public float RecoilStrength => _recoilStrength;
    public float ThrowForce => _throwForce;
    public int MaximumBulletsInMagazine => _maximumBulletsInMagazine;
    public int BulletPerShot => _bulletPerShot;
    public int BulletsAviable => _bulletsAviable;
    public int CurrentAmmo
    {
        get;
    }
    public bool IsLockJump => _isLockJump;
}
