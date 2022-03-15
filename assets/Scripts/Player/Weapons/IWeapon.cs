using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public abstract class IWeapon : MonoBehaviour
{
    [Header("Ammo")]
    [SerializeField] protected int maxAmmo = 12;
    [SerializeField] protected int currentAmmo;
    [SerializeField] protected float reloadTime = 3f;
    protected bool isReloading = false;

    [Header("Animation")]
    [SerializeField] protected Animator _gunAnimator;

    public delegate void AmmoEvent(int currentAmmo, int maxAmmo);
    public event AmmoEvent AmmoChanged;

    protected PhotonView _view;

    public void Start()
    {
        AmmoChanged += (a, b) => { /*Debug.Log("Im being called");*/ };
        currentAmmo = maxAmmo;
        _view = GetComponentInParent<PhotonView>();
        if(_gunAnimator == null) _gunAnimator = GetComponentInChildren<Animator>();
        if (_view.IsMine)
        {
            AmmoChanged.Invoke(currentAmmo, maxAmmo);
        }
    }
        

    public abstract void Shoot(Action onShoot);
    public abstract void Shoot(Vector3 dir, Action onShoot);
    public abstract void Shoot(Vector3 dir);
    public abstract void Shoot();
    public IEnumerator Reload()
    {
        isReloading = true;
        //Debug.Log("Reloading....");
        //_gunAnimator.Play("Reloading");
        _gunAnimator.SetTrigger("Reload");
        _gunAnimator.speed = (1 / reloadTime); // play animation at reload time speed

        yield return new WaitForSeconds(reloadTime);
        _gunAnimator.speed = 1; // reset animation speed

        currentAmmo = maxAmmo;
        AmmoChanged.Invoke(currentAmmo, maxAmmo);
        isReloading = false;
    }
    protected void InvokeEvent(int currentAmmo, int maxAmmo)
    {
        AmmoChanged.Invoke(currentAmmo, maxAmmo);
    }
}
