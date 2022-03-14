using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponSlot : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private GameObject _slotObject;
    [SerializeField] private GameObject _equippedWeapon;
    [SerializeField] private string _weaponName = "Pistol"; //synced

    public GameObject EquippedWeapon { get { return _equippedWeapon; } }

    private IWeapon _weapon; //Reference to IWeapon script
    private PhotonView _view;
    private int _weaponViewId; //synced

    // Start is called before the first frame update
    void Start()
    {
        _view = GetComponent<PhotonView>();
        if (_view.IsMine)
        {
            // owner equip
            EquipWeapon(_weaponName);
        }
        else
        {
            // Local equip
            var equippedWeaponView = PhotonView.Find(_weaponViewId);
            DoEquipObject(equippedWeaponView.gameObject);
            _weapon = equippedWeaponView.gameObject.GetComponent<IWeapon>();
        }
    }

    void Update()
    {
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

    //Equipping
    public void EquipWeapon(string weaponName)
    {
        if (_view.IsMine)
        {
            var spawnedObject = PhotonNetwork.Instantiate("Weapons/" + weaponName, _slotObject.transform.position, _slotObject.transform.rotation);
            _weaponViewId = spawnedObject.GetComponent<PhotonView>().ViewID;
            spawnedObject.TryGetComponent<IWeapon>(out var weapon);
            if(weapon == null)
                Debug.LogError($"Attempted to equip non-weapon in {gameObject.name}! GameObject: {weaponName}");

            DoEquipObject(spawnedObject);
            _weapon = weapon;
            if (spawnedObject != _equippedWeapon)
            {
                // TODO Implement dropping weapon
                var prevEquipped = _equippedWeapon;
                _equippedWeapon = spawnedObject;
                if(prevEquipped != null) PhotonNetwork.Destroy(prevEquipped);
                return;
            }
        }
    }

    private void DoEquipObject(GameObject objectToEquip)
    {
        objectToEquip.transform.SetParent(_slotObject.transform);
        objectToEquip.transform.localPosition = _slotObject.transform.localPosition;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(_weaponName);
            stream.SendNext(_weaponViewId); // isn't synced properly
        }
        else
        {
            this._weaponName = (string) stream.ReceiveNext();
            this._weaponViewId = (int)stream.ReceiveNext();
        }
    }
}
