using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IAttractablePickup : IPickupable
{
    [SerializeField] private float AttractorSpeed = 1;

    public void AttractTowards(GameObject target)
    {
        transform.position =
            Vector3.MoveTowards(transform.position, target.transform.position, AttractorSpeed * Time.deltaTime);
    }
}
