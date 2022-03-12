using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyShooting : MonoBehaviour
{
    [SerializeField] Transform firePoint;
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] float bulletForce = 20f;
    [SerializeField] Animator _anim;
    [SerializeField] float shotCooldown = 1f;

    private float cooldown = 0f;
    private PhotonView _view;

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponentInChildren<Animator>();
        _view = GetComponent<PhotonView>();
    }

    public void Shoot(Vector2 dir, Action onAnimationEnd)
    {
        Shoot(dir);
        StartCoroutine(RunFunctionOnAnimationEnd(onAnimationEnd));
    }

    public void Shoot(Vector2 dir)
    {
        if(Time.time > cooldown)
        {
            _anim?.SetTrigger("Attack");

            CreateBullets(_projectilePrefab.name, firePoint.position, firePoint.rotation, dir, bulletForce);
            _view.RPC("CreateBullets", RpcTarget.Others, _projectilePrefab.name, firePoint.position, firePoint.rotation, dir, bulletForce);

            cooldown = Time.time + shotCooldown;
        }
    }

    public void CreateBullets(string prefabname, Vector3 pos, Quaternion rot, Vector3 firedir, float force)
    {
        GameObject bullet =
            (GameObject)Instantiate(Resources.Load(prefabname), pos, rot);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        Projectile proj = bullet.GetComponent<Projectile>();
        rb.AddForce(firedir * force, ForceMode2D.Impulse);
    }

    public IEnumerator RunFunctionOnAnimationEnd(Action onAnimationEnd)
    {
        while(_anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            yield return new WaitForSeconds(0.1f);
        }
        onAnimationEnd();
    }
}
