using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class IProjectile : MonoBehaviour
{
    [SerializeField] private LayerMask layers = 3968; // By default activates layers 7,8,9,10&11
    
    int damage = 1; 
    private Rigidbody2D _rb;
    private float lifetime = 10f;
    private float deathTime;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        deathTime = Time.time + lifetime;
    }

    public void Init(int dmg, Vector3 dir, float force)
    {
        damage = dmg;
        _rb.AddForce(dir * force, ForceMode2D.Impulse);
    }

    void Update()
    {
        if (Time.time > deathTime)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Called when hitting enemies. Override to customize behaviour.
    /// </summary>
    /// <param name="target">The target hit.</param>
    /// <param name="damage">Amount of damage.</param>
    protected void OnHitTarget(IHurtable target, int damage)
    {
        target.DealDamage(damage);
    }

    /// <summary>
    /// Called when hitting enemies. Override to customize behaviour.
    /// </summary>
    /// <param name="target">The target hit.</param>
    /// <param name="damage">Amount of damage.</param>
    /// <param name="hitdir">The direction of the hit. Used to push enemy back in firedirection.</param>
    protected void OnHitTarget(IHurtable target, int damage, Vector2 hitdir)
    {
        target.DealDamage(damage, hitdir);
    }

    protected void OnTriggerEnter2D(Collider2D col)
    {
        if (((1 << col.gameObject.layer) & layers) != 0) //THIS IS BLACK MAGIC, THAT COMPARES THE LAYERS TO LAYERMASK
        {
            col.TryGetComponent<EnemyHealth>(out var enemyHit);
            if (enemyHit)
            {
                OnHitTarget(enemyHit, damage, _rb.velocity.normalized);
            }
            col.TryGetComponent<PlayerHealth>(out var playerHit);
            if (playerHit)
            {
                OnHitTarget(playerHit, damage);
            }
            Destroy(gameObject);
        }
    }

    protected void OnCollisionEnter2D(Collision2D col)
    {
        if (((1 << col.gameObject.layer) & layers) != 0)
        {
            Destroy(gameObject);
        }
    }
}
