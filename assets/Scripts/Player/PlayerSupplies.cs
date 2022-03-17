using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSupplies : MonoBehaviour
{
    public int Currency { get; private set; }
    //Add ammo types here
    
    public void AddCurrency(int toAdd)
    {
        Currency += toAdd;
    }

    public bool SpendCurrency(int toSpend)
    {
        if (Currency - toSpend < 0) 
            throw new ArgumentOutOfRangeException("Currency cannot be below 0. Please check if player has enough currency before spending.");
        Currency -= toSpend;
        return true;
    }

    public bool CanPlayerAfford(int price)
    {
        return price <= Currency;
    }
}