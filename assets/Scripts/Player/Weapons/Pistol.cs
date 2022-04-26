using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Pistol : IWeapon
{
    [Header("Bullet")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletForce = 20f;
    [SerializeField] int damage = 2;

    [Header("Gun Behaviour")]
    [SerializeField] float secondsBetweenShots = 1f;

    [Header("Camera Shake")]
    [SerializeField] float shakeIntensity = 1f;
    [SerializeField] float shakeTime = .3f;
    [SerializeField] bool decreaseShakeOverTime = false;

    private float cooldown = 0f;
    

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        if (!_view.IsMine)
        {
            return;
        }

    }
    public override void Shoot()
    {
        Shoot(fireDir.transform.up, () => { });
    }

    public override void Shoot(Vector3 dir)
    {
        Shoot(dir, () => { });
    }

    public override void Shoot(Action onShoot)
    {
        Shoot(fireDir.transform.up, onShoot);
    }

    public override void Shoot(Vector3 dir, Action onShoot)
    {
        
        if (isReloading) return;

        if (currentAmmo <= 0)
        {
            Reload();
            return;
        }

        if (Time.time > cooldown)
        {
            currentAmmo--;
            InvokeAmmoChanged(currentAmmo, maxAmmo);
            onShoot();
            ShakeCamera(shakeIntensity, shakeTime, decreaseShakeOverTime);
            //_gunAnimator?.SetTrigger("Attack");

            CreateBullets(bulletPrefab.name, firePoint.transform.position, fireDir.transform.rotation, dir, bulletForce);
            _view.RPC("CreateBullets", RpcTarget.Others, bulletPrefab.name, firePoint.transform.position, fireDir.transform.rotation, dir, bulletForce);

            cooldown = Time.time + secondsBetweenShots;
        }
    }

    [PunRPC]
    void CreateBullets(string prefabname, Vector3 pos, Quaternion rot, Vector3 firedir, float force)
    {
        InvokeOnPlayerShoot();
        GameObject bullet
            = (GameObject) Instantiate(Resources.Load("Projectiles/" + prefabname), pos, rot);
        IProjectile proj = bullet.GetComponent<IProjectile>(); //use for setting dmg and stuff
        proj.Init(damage, firedir, force);
    }
}
