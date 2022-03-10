using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Shooting : MonoBehaviour
{

    private PhotonView view;
    [SerializeField] Transform firePoint;
    public GameObject bulletPrefab;
    private PlayerInput inputActions;
    public float bulletForce = 20f;
    [SerializeField] Animator anim;
    [SerializeField] float Cooldowntime = 1f;
    private float cooldown = 0f;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
        if (!view.IsMine)
        {
            return;
        }
        
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
            anim?.SetTrigger("Attack");
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);
            cooldown = Cooldowntime;
        }
        
    }
}

