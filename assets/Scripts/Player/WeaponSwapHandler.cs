using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwapHandler : MonoBehaviour
{
    [SerializeField] WeaponSlot slot1;


    public void SwapWeaponSlot1(string weaponName)
    {
        slot1.EquipWeapon(weaponName);
    }
}
