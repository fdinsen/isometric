using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class IPickupable : MonoBehaviour
{
    [SerializeField] private Collider2D _collider;
    [SerializeField] private float _lifetimeInSeconds = 300;

    private float _despawnAt;
    private void Awake()
    {
        _despawnAt = Time.time + _lifetimeInSeconds;
    }

    private void FixedUpdate()
    {
        if(Time.time >= _despawnAt)
        {
            Destroy(gameObject);
        }
    }

    public abstract bool DoPickup(GameObject pickupper);

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            if (DoPickup(col.gameObject))
            {
                Destroy(gameObject);
            }
        }
    }
}
