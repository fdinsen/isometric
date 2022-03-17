using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyShooting : MonoBehaviour
{
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject _projectilePrefab;
    [SerializeField] float bulletForce = 20f;
    [SerializeField] Animator _anim;
    [SerializeField] float shotCooldown = 1f;
    [SerializeField] int damage = 10;

    private float cooldown = 0f;
    private PhotonView _view;

    // Start is called before the first frame update
    void Start()
    {
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

            CreateEnemyBullets(_projectilePrefab.name, firePoint.position, firePoint.rotation, dir, bulletForce);
            _view.RPC("CreateEnemyBullets", RpcTarget.Others, _projectilePrefab.name, firePoint.position, firePoint.rotation, dir, bulletForce);

            cooldown = Time.time + shotCooldown;
        }
    }

    [PunRPC]
    public void CreateEnemyBullets(string prefabname, Vector3 pos, Quaternion rot, Vector2 firedir, float force)
    {
        GameObject bullet =
            (GameObject)Instantiate(Resources.Load("Projectiles/" + prefabname), pos, rot);
        IProjectile proj = bullet.GetComponent<IProjectile>();
        proj.Init(damage, firedir, force);
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
