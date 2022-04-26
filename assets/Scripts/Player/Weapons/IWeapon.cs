using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using FMODUnity;
using ExtensionMethods;

[RequireComponent(typeof(AudioPlayer))]
public abstract class IWeapon : MonoBehaviour
{
    [Header("Ammo")]
    [SerializeField] protected AmmoType ammoType;
    [SerializeField] protected int maxAmmo = 12;
    [SerializeField] protected int currentAmmo;
    [SerializeField] protected float reloadTime = 3f;
    protected bool isReloading = false;
    protected PlayerSupplies playerSupply;

    [Header("Animation")]
    [SerializeField] protected Animator _gunAnimator;

    [Header("Audio")]
    [SerializeField] private AudioPlayer _audioPlayer;
    [SerializeField] private EventReference shootSoundEvent;
    [SerializeField] private EventReference reloadSoundEvent;

    [Header("Firing Direction Setup")]
    [SerializeField] protected Transform firePoint;
    [SerializeField] protected Transform fireDir;

    public delegate void AmmoEvent(int currentAmmo, int maxAmmo);
    public event AmmoEvent AmmoChanged;

    public delegate void ShootEvent(OnPlayerShootEventArgs e);
    public static event ShootEvent OnPlayerShoot;

    protected PhotonView _view;
    protected PlayerCameraHandler playerCamHandler;

    public void Start()
    {
        AmmoChanged += (a, b) => { /*Debug.Log("Im being called");*/ };
        currentAmmo = maxAmmo;
        _view = GetComponentInParent<PhotonView>();
        playerSupply = GetComponentInParent<PlayerSupplies>();
        playerCamHandler = GetComponentInParent<PlayerCameraHandler>();
        if(_gunAnimator == null) _gunAnimator = GetComponentInChildren<Animator>();
        if(_audioPlayer == null) _audioPlayer = GetComponentInChildren<AudioPlayer>();
        if (_view.IsMine)
        {
            AmmoChanged.Invoke(currentAmmo, maxAmmo);
        }
    }    

    public abstract void Shoot(Action onShoot);
    public abstract void Shoot(Vector3 dir, Action onShoot);
    public abstract void Shoot(Vector3 dir);
    public abstract void Shoot();

    public void Reload()
    {
        if (!playerSupply.HasAmmo(ammoType)) return;
        PlayReloadSound();
        int ammosupply = playerSupply.GetAmmo(ammoType);
        int amountToReload;
        if( ammosupply - (maxAmmo-currentAmmo) >= 0)
        {
            amountToReload = maxAmmo - currentAmmo;
        }
        else
        {
            amountToReload = ammosupply;
        }
        StartCoroutine(PerformReload(amountToReload));
    }
    protected IEnumerator PerformReload(int amountToReload)
    {
        isReloading = true;
        //Debug.Log("Reloading....");
        //_gunAnimator.Play("Reloading");
        _gunAnimator.SetTrigger("Reload");
        _gunAnimator.speed = (1 / reloadTime); // play animation at reload time speed

        yield return new WaitForSeconds(reloadTime);
        _gunAnimator.speed = 1; // reset animation speed

        currentAmmo += amountToReload;
        playerSupply.UseAmmo(amountToReload, ammoType);
        AmmoChanged.Invoke(currentAmmo, maxAmmo);
        isReloading = false;
    }
    protected void InvokeAmmoChanged(int currentAmmo, int maxAmmo)
    {
        AmmoChanged.Invoke(currentAmmo, maxAmmo);
    }

    protected void InvokeOnPlayerShoot()
    {
        OnPlayerShoot.Invoke(new OnPlayerShootEventArgs(transform.position, firePoint.position, fireDir.rotation.eulerAngles));
    }

    protected void ShakeCamera(float intensity, float time, bool decreaseIntensityOverTime = true)
    {
        playerCamHandler.ShakeCamera(intensity, time, decreaseIntensityOverTime);
    }

    protected void PlayShootSound()
    {
        _audioPlayer.PlayOneshotAttachedRPC(shootSoundEvent.Guid, _view, RpcTarget.All);
    }

    protected void PlayReloadSound()
    {
        _audioPlayer.PlayOneshotAttachedRPC(reloadSoundEvent.Guid, _view, RpcTarget.All);
    }

    #region Event Args
    public class OnPlayerShootEventArgs
    {
        public Vector3 gunPosition;
        public Vector3 gunEndpointPosition;
        public Vector3 aimDirection;

        public OnPlayerShootEventArgs(Vector3 gunPosition, Vector3 gunEndpointPosition, Vector3 aimDirection)
        {
            this.gunPosition = gunPosition;
            this.gunEndpointPosition = gunEndpointPosition;
            this.aimDirection = aimDirection;
        }
    }
    #endregion
}
