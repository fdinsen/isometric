using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSupplies : MonoBehaviour
{
    public int Currency { get; private set; }
    //Add ammo types here

    public delegate void SupplyEvent(int amount);
    public event SupplyEvent CurrencyChanged;

    public void Start()
    {
        CurrencyChanged?.Invoke(Currency);
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            AddCurrency(1);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            SpendCurrency(1);
        }
    }

    public void AddCurrency(int toAdd)
    {
        Currency += toAdd;
        CurrencyChanged?.Invoke(Currency);
    }

    public bool SpendCurrency(int toSpend)
    {
        if (Currency - toSpend < 0) 
            throw new ArgumentOutOfRangeException("Currency cannot be below 0. Please check if player has enough currency before spending.");
        Currency -= toSpend;
        CurrencyChanged?.Invoke(Currency);
        return true;
    }

    public bool CanPlayerAfford(int price)
    {
        return price <= Currency;
    }
}