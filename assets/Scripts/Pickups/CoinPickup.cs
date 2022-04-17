using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : IAttractablePickup
{
    [SerializeField] int coinValue;

    public override bool DoPickup(GameObject pickupper)
    {
        var supplies = pickupper.GetComponent<PlayerSupplies>();
        if (!supplies) { Debug.LogError("Player had no PlayerSupplies object to pick up coin."); return false; }
        supplies.AddCurrency(coinValue);
        return true;
    }

}
