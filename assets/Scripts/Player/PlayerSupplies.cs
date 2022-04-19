using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSupplies : MonoBehaviour
{
    public int Currency { get; private set; }
    [SerializeField] private int _smallAmmo;
    [SerializeField] private int _mediumAmmo;
    [SerializeField] private int _heavyAmmo;

    public delegate void SupplyEvent(int amount);
    public delegate void AmmoSupplyEvent(int amount, AmmoType type);
    public event SupplyEvent CurrencyChanged;
    public event AmmoSupplyEvent AmmoChanged;

    #region Unity Setup
    public void Start()
    {
        CurrencyChanged?.Invoke(Currency);
        //AmmoChanged += (ammo, type) => Debug.Log(type + ": " + ammo);
        AmmoChanged?.Invoke(_smallAmmo, AmmoType.SMALL);
        AmmoChanged?.Invoke(_mediumAmmo, AmmoType.MEDIUM);
        AmmoChanged?.Invoke(_heavyAmmo, AmmoType.HEAVY);
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            AddCurrency(1);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            SpendCurrency(1);
        }
    }
    #endregion

    #region Ammo
    public void AddAmmo(int toAdd, AmmoType type)
    {
        switch(type)
        {
            case AmmoType.SMALL:
                _smallAmmo += toAdd;
                AmmoChanged?.Invoke(_smallAmmo, type);
                break;
            case AmmoType.MEDIUM:
                _mediumAmmo += toAdd;
                AmmoChanged?.Invoke(_mediumAmmo, type);
                break;
            case AmmoType.HEAVY:
                _heavyAmmo += toAdd;
                AmmoChanged?.Invoke(_heavyAmmo, type);
                break;
            default:
                throw new NotImplementedException($"Attempted to add ammo to unimplemented AmmoType {type}, in PlayerSupplies class.");
        }
    }

    public bool UseAmmo(int toUse, AmmoType type)
    {
        bool result;
        switch (type)
        {
            case AmmoType.SMALL:
                (result, _smallAmmo) = DoUseAmmo(toUse, _smallAmmo, type);
                return result;
            case AmmoType.MEDIUM:
                (result, _mediumAmmo) = DoUseAmmo(toUse, _mediumAmmo, type);
                return result;
            case AmmoType.HEAVY:
                (result, _heavyAmmo) = DoUseAmmo(toUse, _heavyAmmo, type);
                return result;
            default:
                throw new NotImplementedException($"Attempted to add ammo to unimplemented AmmoType {type}, in PlayerSupplies class.");
        }
    }

    public bool HasAmmo(AmmoType type)
    {
        return type switch
        {
            AmmoType.SMALL => _smallAmmo != 0,
            AmmoType.MEDIUM => _mediumAmmo != 0,
            AmmoType.HEAVY => _heavyAmmo != 0,
            _ => throw new NotImplementedException($"Attempted to check ammo of unimplemented AmmoType {type}, in PlayerSupplies class."),
        };
    }
    
    public int GetAmmo(AmmoType type)
    {
        return type switch
        {
            AmmoType.SMALL => _smallAmmo,
            AmmoType.MEDIUM => _mediumAmmo,
            AmmoType.HEAVY => _heavyAmmo,
            _ => throw new NotImplementedException($"Attempted to get ammo from unimplemented AmmoType {type}, in PlayerSupplies class."),
        };
    }

    public void SetAmmo(int amount, AmmoType type)
    {
        switch (type)
        {
            case AmmoType.SMALL:
                _smallAmmo = amount;
                break;
            case AmmoType.MEDIUM:
                _mediumAmmo = amount;
                break;
            case AmmoType.HEAVY:
                _heavyAmmo = amount;
                break;
            default:
                throw new NotImplementedException($"Attempted to set ammo of unimplemented AmmoType {type}, in PlayerSupplies class.");
        }
    }

    private (bool, int) DoUseAmmo(int toUse, int ammo, AmmoType type)
    {
        if (ammo - toUse < 0)
        {
            return (false, ammo);
        }
        ammo -= toUse;
        AmmoChanged?.Invoke(ammo, type);
        return (true, ammo);
    }
    #endregion

    #region Currency
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
    #endregion

    #region Helper Functions
    public static PlayerSupplies GetMyPlayerSupplies()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players)
        {
            player.TryGetComponent(out PlayerSupplies myPlayer);
            if (myPlayer == null) { break; }
            return myPlayer;
        }
        Debug.LogError("Error: Could not find active player!");
        return null;
    }
    #endregion
}

public enum AmmoType
{
    SMALL, MEDIUM, HEAVY
}