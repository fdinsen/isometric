using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

[RequireComponent(typeof(Slider))]
public class HealthBarManager : MonoBehaviour
{
    private PlayerHealth _player;
    private Slider _slider;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Setup_HealthbarManager());
    }

    void SetHealth(int health, int maxHealth)
    {
        _slider.maxValue = maxHealth;
        _slider.value = health;
    }

    private PlayerHealth GetMyPlayerHealth()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players)
        {
            player.TryGetComponent(out PlayerHealth myPlayer);
            if (myPlayer == null) { break; }
            return myPlayer;
        }
        Debug.LogError("Error: Health Bar could not find active player!");
        return null;
    }

    private IEnumerator Setup_HealthbarManager()
    {
        yield return new WaitForEndOfFrame();
        _player = GetMyPlayerHealth();
        _slider = GetComponent<Slider>();

        _player.HealthChanged += SetHealth;
    }
}

