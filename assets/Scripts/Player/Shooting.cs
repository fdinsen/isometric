using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Shooting : MonoBehaviour
{ 
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletForce = 20f;
    [SerializeField] Animator anim;
    [SerializeField] float Cooldowntime = 1f;
    
    private float cooldown = 0f;
    private PlayerInput inputActions;
    private TraumaManager _traumaManager;
    private PhotonView _view;

    private void Start()
    {
        if (gameObject.CompareTag("OtherPlayer"))
        {
            return;
        }
        _view = GetComponent<PhotonView>();
        GameObject.FindGameObjectWithTag("TraumaManager").TryGetComponent(out _traumaManager);

        inputActions = new PlayerInput();
        inputActions.Combat.Enable();
        inputActions.Combat.Shooting.performed += ctx => Shoot();
        anim = GetComponentInChildren<Animator>();
    }
    private void Update()
    {
        if (cooldown > 0f)
        {
            cooldown -= Time.deltaTime;
        }
    }


    void Shoot()
    {
        if (cooldown <= 0f)
        {
            _traumaManager?.AddTrauma(0.4f);
            anim?.SetTrigger("Attack");

            CreateBullets(bulletPrefab.name, firePoint.position, firePoint.rotation, firePoint.up, bulletForce, gameObject.GetInstanceID());
            _view.RPC("CreateBullets", RpcTarget.Others, bulletPrefab.name, firePoint.position, firePoint.rotation, firePoint.up, bulletForce, gameObject.GetInstanceID());
            
            cooldown = Cooldowntime;
        }
    }

    [PunRPC]
    void CreateBullets(string prefabname, Vector3 pos, Quaternion rot, Vector3 firedir, float force, int ownerid)
    {
        GameObject bullet 
            = (GameObject) Instantiate(Resources.Load(prefabname), pos, rot);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        Projectile proj = bullet.GetComponent<Projectile>(); //use for setting dmg and stuff
        rb.AddForce(firedir * force, ForceMode2D.Impulse);
    }
}

