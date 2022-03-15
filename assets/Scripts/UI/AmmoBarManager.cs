using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

[RequireComponent(typeof(Slider))]

public class AmmoBarManager : MonoBehaviour
{
    private WeaponSlot _weaponSlot;
    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        _slider.value = 1f;
        _weaponSlot = GetMyWeaponSlot();
        _weaponSlot.WeaponSwapped += _ => { ConnectToWeapon(); /*Debug.Log("Connected ABM");*/ };
    }

    void SetAmmo(int currentAmmo, int maxAmmo)
    {
        _slider.maxValue = maxAmmo;
        _slider.value = currentAmmo;
    }

    void ConnectToWeapon()
    {
        if (_weaponSlot == null) Debug.Log("Null");
        _weaponSlot.WeaponScript.AmmoChanged += SetAmmo;
    }

    private WeaponSlot GetMyWeaponSlot()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players)
        {
            player.TryGetComponent(out WeaponSlot myPlayer);
            if (myPlayer == null) { break; }
            return myPlayer;
        }
        Debug.LogError("Error: Ammo Bar could not find active player!");
        return null;
    }
}