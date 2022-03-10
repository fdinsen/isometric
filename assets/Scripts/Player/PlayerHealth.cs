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

    private Animator _anim;

    void Start()
    {
        //Testing code
        HealthChanged += (a, b) => Debug.Log("");
        var p = new PlayerInput();
        p.Movement.Enable();
        p.Movement.Test.performed += ctx => { DealDamage(10); };
        //Testing code end

        _anim = GetComponentInChildren<Animator>();
        PlayerDied += (a, b) => StartCoroutine(Die());
        Health = 100;
        HealthChanged.Invoke(Health, maxHealth);
    }

    public void DealDamage(int amnt)
    {
        Health -= amnt;
        _anim.SetTrigger("Hurt");
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

    public IEnumerator Die()
    {
        _anim.SetBool("Dead", true);
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}
