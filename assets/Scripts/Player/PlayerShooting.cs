using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PlayerController))]
public class PlayerShooting : MonoBehaviour
{ 
    [SerializeField] WeaponSlot _slot;
    
    private PlayerController _player;

    private void Start()
    {
        if (gameObject.CompareTag("OtherPlayer"))
        {
            return;
        }
        _player = GetComponent<PlayerController>();


        _player.PlayerInput.Combat.Enable();
        _player.PlayerInput.Combat.Shooting.performed += ctx => Shoot();
        
        TryGetComponent<PlayerHealth>(out var pHealth);
        if(pHealth != null)
        {
            pHealth.PlayerDied += (a, b) => _player.PlayerInput.Combat.Disable();
        }
    }

    private void OnDisable()
    {
        _player.PlayerInput.Combat.Disable();
    }


    void Shoot()
    {
        _slot.ShootWeapon(() => _player.PlayAttackAnimation());
    }
}
