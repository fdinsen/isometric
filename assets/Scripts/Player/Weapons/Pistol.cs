using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Pistol : IWeapon
{
    [Header("Firing Direction Setup")]
    [SerializeField] GameObject firePoint;
    [SerializeField] GameObject fireDir;

    [Header("Bullet")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletForce = 20f;
    [SerializeField] int damage = 1;

    [Header("Gun Behaviour")]
    [SerializeField] float secondsBetweenShots = 1f;

    [Header("Animation")]
    [SerializeField] Animator _gunAnimator;

    private float cooldown = 0f;
    private TraumaManager _traumaManager;
    private PhotonView _view;

    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.CompareTag("OtherPlayer"))
        {
            return;
        }
        GameObject.FindGameObjectWithTag("TraumaManager")?.TryGetComponent(out _traumaManager);
        _view = GetComponentInParent<PhotonView>();
        _gunAnimator = GetComponentInChildren<Animator>();
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
        if (Time.time > cooldown)
        {
            onShoot();
            _traumaManager?.AddTrauma(0.4f);
            _gunAnimator?.SetTrigger("Attack");

            CreateBullets(bulletPrefab.name, firePoint.transform.position, fireDir.transform.rotation, dir, bulletForce);
            _view.RPC("CreateBullets", RpcTarget.Others, bulletPrefab.name, firePoint.transform.position, fireDir.transform.rotation, dir, bulletForce);

            cooldown = Time.time + secondsBetweenShots;
        }
    }

    [PunRPC]
    void CreateBullets(string prefabname, Vector3 pos, Quaternion rot, Vector3 firedir, float force)
    {
        GameObject bullet
            = (GameObject)Instantiate(Resources.Load(prefabname), pos, rot);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        Projectile proj = bullet.GetComponent<Projectile>(); //use for setting dmg and stuff
        proj.SetDamage(damage);
        rb.AddForce(firedir * force, ForceMode2D.Impulse);
    }
}
