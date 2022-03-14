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

    public IWeapon WeaponScript { get; private set; } //Reference to IWeapon script
    private PhotonView _view;
    private int _weaponViewId; //synced

    public delegate void WeaponEvent(string weaponName);
    public event WeaponEvent WeaponSwapped;

    // Start is called before the first frame update
    void Start()
    {
        WeaponSwapped += wname => { };
        _view = GetComponent<PhotonView>();
        if (_view.IsMine)
        {
            // owner equip
            PerformEquip(CreateWeaponAsOwner(_weaponName));
            WeaponSwapped.Invoke(_weaponName);
        }
        else
        {
            // Local equip
            ProporgateWeaponSwap();
        }
    }

    void Update()
    {
    }

    //Shooting
    public void ShootWeapon(Action onShoot)
    {
        WeaponScript.Shoot(onShoot);
    }
    public void ShootWeapon(Vector3 dir, Action onShoot)
    {
        WeaponScript.Shoot(dir, onShoot);
    }
    public void ShootWeapon(Vector3 dir)
    {
        WeaponScript.Shoot(dir);
    }
    public void ShootWeapon()
    {
        WeaponScript.Shoot();
    }

    //Equipping
    public void EquipWeapon(string weaponName)
    {
        if (_view.IsMine)
        {
            PerformEquip(CreateWeaponAsOwner(weaponName));
            WeaponSwapped.Invoke(weaponName);
        }
    }

    [PunRPC]
    public void ProporgateWeaponSwap()
    {
        var equippedWeaponView = PhotonView.Find(_weaponViewId);
        PerformEquip(equippedWeaponView.gameObject);
    }

    private GameObject CreateWeaponAsOwner(string weaponName)
    {
        var spawnedObject = PhotonNetwork.Instantiate("Weapons/" + weaponName, _slotObject.transform.position, _slotObject.transform.rotation);
        _weaponViewId = spawnedObject.GetComponent<PhotonView>().ViewID;
        return spawnedObject;
    }

    private void PerformEquip(GameObject weaponObject)
    {
        weaponObject.TryGetComponent<IWeapon>(out var weapon);
        if (weapon == null)
            Debug.LogError($"Attempted to equip non-weapon in {gameObject.name}! GameObject: {weaponObject.name}");

        AttachObject(weaponObject);
        WeaponScript = weapon;
        if (weaponObject != _equippedWeapon && _view.IsMine)
        {
            // TODO Implement dropping weapon
            var prevEquipped = _equippedWeapon;
            if (prevEquipped != null) PhotonNetwork.Destroy(prevEquipped);
        }
        _equippedWeapon = weaponObject;
    }

    private void AttachObject(GameObject objectToAttach)
    {
        objectToAttach.transform.SetParent(_slotObject.transform);
        objectToAttach.transform.localPosition = _slotObject.transform.localPosition;
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
