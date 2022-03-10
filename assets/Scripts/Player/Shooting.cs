using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    [SerializeField] Transform firePoint;
    public GameObject bulletPrefab;
    private PlayerInput inputActions;
    public float bulletForce = 20f;


    private void Awake()
    {
        inputActions = new PlayerInput();
        inputActions.Combat.Enable();
        inputActions.Combat.Shooting.performed += ctx => Shoot();
    }
  

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);
    }
}

