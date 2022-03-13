using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwapHandler : MonoBehaviour
{
    [SerializeField] WeaponSlot slot1;


    public void SwapWeaponSlot1(string weaponName)
    {
        GameObject newWeapon = Resources.Load<GameObject>("Weapons/" + weaponName);
        var spawned = Instantiate(newWeapon);
        slot1.EquippedWeapon = spawned;
    }
}
