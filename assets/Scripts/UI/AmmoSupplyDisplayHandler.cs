using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AmmoSupplyDisplayHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _lightText;
    [SerializeField] private TextMeshProUGUI _mediumText;
    [SerializeField] private TextMeshProUGUI _heavyText;
    private PlayerSupplies _player;
    // Start is called before the first frame update
    void Start()
    {
        if (_lightText == null) Debug.LogError("Light Text not set in AmmoSupplyDisplayHandler");
        if (_mediumText == null) Debug.LogError("Medium Text not set in AmmoSupplyDisplayHandler");
        if (_heavyText == null) Debug.LogError("Heavy Text not set in AmmoSupplyDisplayHandler");
        _player = PlayerSupplies.GetMyPlayerSupplies();
        _player.AmmoChanged += SetAmmo;
        SetAmmo(_player.GetAmmo(AmmoType.SMALL), AmmoType.SMALL); // initializes
        SetAmmo(_player.GetAmmo(AmmoType.MEDIUM), AmmoType.MEDIUM); // initializes
        SetAmmo(_player.GetAmmo(AmmoType.HEAVY), AmmoType.HEAVY); // initializes
    }

    private void SetAmmo(int amount, AmmoType type)
    {
        switch (type)
        {
            case AmmoType.SMALL:
                _lightText.text = amount.ToString();
                break;
            case AmmoType.MEDIUM:
                _mediumText.text = amount.ToString();
                break;
            case AmmoType.HEAVY:
                _heavyText.text = amount.ToString();
                break;
            default:
                throw new NotImplementedException($"Attempted to check ammo of unimplemented AmmoType {type}, in PlayerSupplies class.");
        };
    }
}
