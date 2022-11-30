using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GunHolder : MonoBehaviour
{
    private PlayerController2d _playerController2D;
    private SpriteRenderer _spriteRenderer;
    private Weapon _weapon;
    private IEnumerator _reloadWeapon;
    private bool _isReloading;

    [Header("Noise")]
    [SerializeField] private float _activeNoiseTime = .1f;
    [SerializeField] private NoiseMaker _noiseMaker;
    [SerializeField] private float _pickUpSoundVolume = .35f;
    [SerializeField] private float _pickUpNoiseRadius = .35f;
    private IEnumerator _noiseDisabler;

    [SerializeField] private float _throwOutAngularVelocity = 245;

    [Header("Animations")]
    [SerializeField] private string _idleAnimation;
    [SerializeField] private string _pistiolRecoilAnimation;
    [SerializeField] private string _riffleRecoilAnimation;
    [SerializeField] private string _shotgunRecoilAnimation;
    public Weapon Weapon
    {
        get => _weapon;
        set
        {
            _weapon = value;
            _playerController2D.UpdateWeaponAnimation();

            if (value == null)
            {
                _isReloading = false;
            }
        }
    }
    public NoiseMaker NoiseMaker => _noiseMaker;
    public bool IsCanThrowWeapon
    {
        get;
        set;
    }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerController2D = transform.parent.GetComponent<PlayerController2d>();
    }

    private void Update()
    {
        if (Weapon != null)
        {
            if (Input.GetMouseButtonUp(1))
            {
                IsCanThrowWeapon = true;
            }

            if (Input.GetMouseButtonDown(0))
            {
                Weapon.ResetShotDelayTime();
            }

            if (Input.GetMouseButton(0) && !_isReloading)
            {
                Weapon.ShotFrom(_playerController2D);
            }
            
            if (Input.GetMouseButtonDown(1) && IsCanThrowWeapon)
            {
                Weapon.ThrowOut(_throwOutAngularVelocity);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                ReloadWeaponAfter(Weapon.WeaponInfo.ReloadTime);
            }
        }
    }

    public void LoadWeapon(Weapon weapon)
    {
        _spriteRenderer.sprite = weapon.WeaponInfo.Sprite;
        weapon.gameObject.SetActive(false);
        Weapon = weapon;
        _playerController2D.IsLockJump = weapon.WeaponInfo.IsLockJump;
        NoiseMaker.PlayRandomAudioWithCreateNoise(weapon.WeaponInfo.PickUpSounds, _pickUpSoundVolume, _pickUpNoiseRadius);
    }

    public void ClearWeapon()
    {
        StopReloadWeapon();
        _playerController2D.IsLockJump = false;
        _spriteRenderer.sprite = null;
        IsCanThrowWeapon = false;
        Weapon.GunHolder = null;
        Weapon = null;
        Debug.Log("Clear weapon");
    }

    #region Reload

    public void ReloadWeaponAfter(float time)
    {
        if (!_isReloading)
        {
            _isReloading = true;

            StopReloadWeapon();

            _reloadWeapon = ReloadWeapon(time);
            StartCoroutine(_reloadWeapon);
        }
    }

    private void StopReloadWeapon()
    {
        if (_reloadWeapon != null)
        {
            StopCoroutine(_reloadWeapon);
        }
    }

    private IEnumerator ReloadWeapon(float time)
    {
        Debug.Log($"{gameObject.name} realoading {Weapon.gameObject.name} for {Weapon.WeaponInfo.ReloadTime}");
        yield return new WaitForSecondsRealtime(time);

        if (Weapon.BulletsAviable >= Weapon.WeaponInfo.MaximumBulletsInMagazine)
        {
            Weapon.BulletsAviable -= Weapon.WeaponInfo.MaximumBulletsInMagazine;
            Weapon.CurrentAmmoInMagazine += Weapon.WeaponInfo.MaximumBulletsInMagazine;
        }
        else
        {
            Weapon.CurrentAmmoInMagazine += Weapon.BulletsAviable;
            Weapon.BulletsAviable = 0;
        }

        _isReloading = false;
    }

    #endregion

    #region Noise

    public void StartNoiseDisabler()
    {
        StopNoiseDisabler();

        _noiseDisabler = NoiseDisabler();
        StartCoroutine(_noiseDisabler);
    }

    private void StopNoiseDisabler()
    {
        if (_noiseDisabler != null)
        {
            StopCoroutine(_noiseDisabler);
        }
    }

    private IEnumerator NoiseDisabler()
    {
        yield return new WaitForSeconds(_activeNoiseTime);
        _noiseMaker.Noise.enabled = false;
    }

    #endregion


}
