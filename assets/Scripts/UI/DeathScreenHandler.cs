using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScreenHandler : MonoBehaviour
{
    [SerializeField] GameObject DeathScreenUIElements; 

    private PlayerHealth player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        player.PlayerDied += (a, b) => OnPlayerDeath();
    }

    private void OnPlayerDeath()
    {
        DeathScreenUIElements.SetActive(true);
    }
}
