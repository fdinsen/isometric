using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class IPickupable : MonoBehaviour
{
    [SerializeField] private Collider2D _collider;

    public abstract bool DoPickup(GameObject pickupper);

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            if (DoPickup(col.gameObject))
            {
                Destroy(gameObject);
                //RPC PROPOERGATE PICKUP
            }
        }
    }
}
