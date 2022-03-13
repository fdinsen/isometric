using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using CodeMonkey.Utils;
using CodeMonkey;

public class Handgun : IWeapon
{
    [Header("Firing Direction Setup")]
    [SerializeField] Transform firePoint;
    [SerializeField] Transform fireDir;

    //[Header("Bullet")]
    //[SerializeField] GameObject bulletPrefab;
    //[SerializeField] float bulletForce = 20f;
    //[SerializeField] int damage = 1;

    [Header("Projectile Tracer")]
    [SerializeField] private Material weaponTracerMaterial;
    [SerializeField] private float tracerWidth = .25f;
    [SerializeField] private float tracerWorldSize = 8f;
    [SerializeField] private float tracerLifetime = .1f;

    [Header("Bullet Settings")]
    [SerializeField] int damage = 1;
    [SerializeField] float range = 100f;

    [Header("Flash")]
    [SerializeField] private Sprite shootFlashSprite; 

    [Header("Gun Behaviour")]
    [SerializeField] float secondsBetweenShots = 1f;

    [Header("Animation")]
    [SerializeField] Animator _gunAnimator;

    private float cooldown = 0f;
    private TraumaManager _traumaManager;
    private PhotonView _view;

    //public delegate void ShootEvent(Vector3 loc, Vector3 target);
    //public event ShootEvent OnShoot;

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
        Shoot(UtilsClass.GetMouseWorldPosition(), () => { });
    }

    public override void Shoot(Vector3 target)
    {
        Shoot(target, () => { });
    }

    public override void Shoot(Action onShoot)
    {
        Shoot(UtilsClass.GetMouseWorldPosition(), onShoot);
    }

    public override void Shoot(Vector3 target, Action onShoot)
    {
        if (Time.time > cooldown)
        {
            onShoot();
            CreateWeaponTracer(firePoint.position, target);
            CreateShootFlash(firePoint.position);
            _traumaManager?.AddTrauma(0.2f);
            _gunAnimator?.SetTrigger("Attack");

            //CreateBullets(bulletPrefab.name, firePoint.position, firePoint.rotation, dir, bulletForce);
            //_view.RPC("CreateBullets", RpcTarget.Others, bulletPrefab.name, firePoint.position, firePoint.rotation, dir, bulletForce);

            cooldown = Time.time + secondsBetweenShots;
        }
    }

    [PunRPC]
    void CreateWeaponTracer(Vector3 fromPosition, Vector3 targetPosition)
    {
        Vector3 dir = (targetPosition - fromPosition).normalized;
        //Raycast
        RaycastHit2D hit = Physics2D.Raycast(fromPosition, dir);
        if(hit)
        {
            targetPosition = hit.collider.transform.position;
            if (LayerMask.LayerToName(hit.collider.gameObject.layer) == "Enemy")
            {
                hit.collider.TryGetComponent<EnemyHealth>(out var enemyHit);
                if (enemyHit)
                {
                    enemyHit.DealDamage(damage, dir);
                }
            }
        }

        //Visuals
        float eulerZ = UtilsClass.GetAngleFromVectorFloat(dir) - 90;
        float distance = Vector3.Distance(fromPosition, targetPosition);
        Vector3 tracerSpawnPosition = fromPosition + dir * distance * .5f;
        Material tmpWeaponTracerMaterial = new Material(weaponTracerMaterial);
        tmpWeaponTracerMaterial.SetTextureScale("_MainTex", new Vector2(1f, distance / tracerWorldSize));
        World_Mesh worldMesh = World_Mesh.Create(tracerSpawnPosition, eulerZ, tracerWidth, distance, tmpWeaponTracerMaterial, null, 10000);

        int frame = 0;
        float framerate = .016f;
        float timer = framerate;
        worldMesh.SetUVCoords(new World_Mesh.UVCoords(0,0,16,256));
        FunctionUpdater.Create(() =>
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                frame++;
                timer += framerate;
                if(frame >= 4)
                {
                    worldMesh.DestroySelf();
                    return true;
                }else
                {
                    worldMesh.SetUVCoords(new World_Mesh.UVCoords(16 * frame, 0, 16, 256));
                }
            }
            return false;
        });
    }

    [PunRPC]
    void CreateShootFlash(Vector3 spawnPosition)
    {
        World_Sprite worldSprite = World_Sprite.Create(spawnPosition, shootFlashSprite);
        FunctionTimer.Create(worldSprite.DestroySelf, .1f);
    }
}
