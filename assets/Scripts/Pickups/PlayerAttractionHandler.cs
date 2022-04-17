using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttractionHandler : MonoBehaviour
{
    [SerializeField] private float _attractionRange = 10;
    [SerializeField] private LayerMask _attractedLayers;

    void FixedUpdate()
    {
        if (gameObject.CompareTag("OtherPlayer")) return;
        Collider2D[] results = Physics2D.OverlapCircleAll(transform.position, _attractionRange, _attractedLayers);
        if(results.Length > 0)
        {
            foreach (var obj in results)
            {
                obj.GetComponent<IAttractablePickup>().AttractTowards(gameObject);
            }
        }
    }
}
