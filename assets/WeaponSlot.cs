using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponSlot : MonoBehaviour
{
    [SerializeField] private GameObject _equippedWeapon;
    [SerializeField] private string _defaultWeaponName = "Pistol";

    public GameObject EquippedWeapon { get { return _equippedWeapon; } set { SetEquippedWeapon(value); } }

    private IWeapon _weapon; //Reference to IWeapon script

    // Start is called before the first frame update
    void Start()
    {
        if(_equippedWeapon == null)
        {
            EquipDefaultWeapon();
        }
        else if (_weapon == null)
        {
            SetEquippedWeapon(_equippedWeapon);
        }
    }

    //Shooting
    public void ShootWeapon(Action onShoot)
    {
        _weapon.Shoot(onShoot);
    }
    public void ShootWeapon(Vector3 dir, Action onShoot)
    {
        _weapon.Shoot(dir, onShoot);
    }
    public void ShootWeapon(Vector3 dir)
    {
        _weapon.Shoot(dir);
    }
    public void ShootWeapon()
    {
        _weapon.Shoot();
    }


    private void SetEquippedWeapon(GameObject toEquip)
    {
        toEquip.TryGetComponent<IWeapon>(out var weapon);
        if (weapon != null)
        {
            _weapon = weapon;
            if(toEquip != _equippedWeapon)
            {
                // TODO Implement dropping weapon
                var prevEquipped = _equippedWeapon;
                _equippedWeapon = toEquip;
                Destroy(prevEquipped);
                DoEquip();
            }
            return;
        }
        Debug.LogError($"Attempted to equip non-weapon in {gameObject.name}! GameObject: {toEquip.name}");       
    }

    private void DoEquip()
    {
        _equippedWeapon.transform.SetParent(gameObject.transform);
        _equippedWeapon.transform.localPosition = gameObject.transform.localPosition;
    }

    private void EquipDefaultWeapon()
    {
        GameObject newWeapon = Resources.Load<GameObject>("Weapons/" + _defaultWeaponName);
        var spawned = Instantiate(newWeapon);
        
        SetEquippedWeapon(spawned);
    }
}
