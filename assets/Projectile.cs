using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{

    [SerializeField] private LayerMask layers;
    [SerializeField] int dmg = 1;

    private Rigidbody2D _rb;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (((1 << col.gameObject.layer) & layers) != 0) //THIS IS BLACK MAGIC, THAT COMPARES THE LAYERS TO LAYERMASK
        {
            col.TryGetComponent<EnemyHealth>(out var enemyHit);
            if(enemyHit)
            {
                enemyHit.DealDamage(dmg, _rb.velocity.normalized);
            }
            col.TryGetComponent<PlayerHealth>(out var playerHit);
            if(playerHit)
            {
                playerHit.DealDamage(dmg);
            }
            Destroy(gameObject);
        }
    }

}
