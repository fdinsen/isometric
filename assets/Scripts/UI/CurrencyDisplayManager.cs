using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrencyDisplayManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    private PlayerSupplies _player;
    // Start is called before the first frame update
    void Start()
    {
        if (_text == null) _text = GetComponentInChildren<TextMeshProUGUI>();
        _player = PlayerSupplies.GetMyPlayerSupplies();
        _player.CurrencyChanged += SetCurrency;
        SetCurrency(_player.Currency); // initializes
    }

    void SetCurrency(int currency)
    {
        _text.text = currency.ToString();
    }
}
