using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] int maxHealth = 100;
    public int Health { get; private set; }
    public delegate void HealthEvent(int health, int maxHealth);
    public event HealthEvent HealthChanged;
    public event HealthEvent PlayerDied;

    void Start()
    {
        //Testing code
        PlayerDied += (a, b) => Debug.Log("Player Died");
        HealthChanged += (a, b) => Debug.Log("");
        var p = new PlayerInput();
        p.Movement.Enable();
        p.Movement.Test.performed += ctx => DealDamage(10);
        //Testing code end

        Health = 100;
        HealthChanged.Invoke(Health, maxHealth);
    }

    public void DealDamage(int amnt)
    {
        Health -= amnt;
        HealthChanged.Invoke(Health, maxHealth);
        if(Health <= 0)
        {
            PlayerDied.Invoke(Health, maxHealth);
        }
    }

    public void Heal(int amnt)
    {
        Health += amnt;
        if(Health > maxHealth)
        {
            Health = maxHealth;
        }
        HealthChanged.Invoke(Health, maxHealth);
    }
}
