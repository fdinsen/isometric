using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class IWeapon : MonoBehaviour
{
    [Header("Ammo")]
    [SerializeField] protected int maxAmmo = 12;
    [SerializeField] protected int currentAmmo;
    [SerializeField] protected float reloadTime = 3f;
    protected bool isReloading = false;

    public delegate void AmmoEvent(int currentAmmo, int maxAmmo);
    public event AmmoEvent AmmoChanged;

    public void Start()
    {
        AmmoChanged += (a, b) => { Debug.Log("Im being called"); };
        currentAmmo = maxAmmo;
    }
        

    public abstract void Shoot(Action onShoot);
    public abstract void Shoot(Vector3 dir, Action onShoot);
    public abstract void Shoot(Vector3 dir);
    public abstract void Shoot();
    public IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading....");

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        AmmoChanged.Invoke(currentAmmo, maxAmmo);
        isReloading = false;
    }
    protected void InvokeEvent(int currentAmmo, int maxAmmo)
    {
        AmmoChanged.Invoke(currentAmmo, maxAmmo);
    }
    
}
