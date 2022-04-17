using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class IProjectile : MonoBehaviour
{
    [SerializeField] private LayerMask layers = 3968; // By default activates layers 7,8,9,10&11
    [SerializeField] private Animator _anim;

    int damage = 1; 
    private Rigidbody2D _rb;
    private float lifetime = 10f;
    private float deathTime;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        deathTime = Time.time + lifetime;
    }

    public void Init(int dmg, Vector3 dir, float force)
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        damage = dmg;
        _rb.AddForce(dir * force, ForceMode2D.Impulse);
        
        var targetRotation = Quaternion.LookRotation(dir, transform.up);
        targetRotation = Quaternion.Euler(0, 0, targetRotation.eulerAngles.z);
        transform.rotation = targetRotation;
    }

    void Update()
    {
        if (Time.time > deathTime)
        {
            StartCoroutine(DestroyDelf());
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
        _anim?.SetTrigger("Exploding");
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
            StartCoroutine(DestroyDelf());
        }
    }

    protected void OnCollisionEnter2D(Collision2D col)
    {
        if (((1 << col.gameObject.layer) & layers) != 0)
        {
            _anim?.SetTrigger("Exploding");
            StartCoroutine(DestroyDelf());
        }
    }

    protected IEnumerator DestroyDelf()
    {
        _rb.bodyType = RigidbodyType2D.Static;
        if (_anim == null) Destroy(gameObject);
        while (true)
        {
            if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Gone")) break;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }
}
