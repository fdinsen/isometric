using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PlayerController))]
public class PlayerHealth : MonoBehaviourPunCallbacks, IPunObservable, IHurtable
{
    [SerializeField] int maxHealth = 100;
    public int Health { get; private set; }
    public int MaxHealth { get { return maxHealth; } private set { maxHealth = value; } }
    public delegate void HealthEvent(int health, int maxHealth);
    public event HealthEvent HealthChanged;
    public event HealthEvent PlayerDied;

    private PlayerController _player;
    //private PhotonView view;

    void Update()
    {
        //Debug.Log(gameObject.tag + " health: " + Health);
    }
    void Start()
    {
        HealthChanged += (a, b) => { }; // do nothing action
        //Testing code end
        _player = GetComponentInChildren<PlayerController>();
        PlayerDied += (a, b) => StartCoroutine(Die());
        Health = 100;
        HealthChanged.Invoke(Health, maxHealth);
    }


    public void DealDamage(int amnt)
    {
        Health -= amnt;
        _player.PlayHurtAnimation();
        HealthChanged.Invoke(Health, maxHealth);
        if(Health <= 0)
        {
            PlayerDied.Invoke(Health, maxHealth);
        }
    }

    public void DealDamage(int dmg, Vector2 hitdir)
    {
        Debug.LogWarning("You are using DealDamage method that includes hitdir on PlayerHealth, but PlayerHealth does not support hitdir. Method will still deal damage, but no force is applied.");
        DealDamage(dmg);
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
        _player.PlayDeathAnimation();
        yield return new WaitForSeconds(1f);
       
        //gameObject.SetActive(false);
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
            GameManager.Instance?.MarkPlayerAsDead();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting) // I am the owner of this object
        {
            stream.SendNext(Health); // We share the health over the network
            stream.SendNext(maxHealth); // always send in the same order we recieve
        }
        else // Another client is the owner of this object
        {
            this.Health = (int)stream.ReceiveNext(); // we recieve the health over the network
            this.maxHealth = (int)stream.ReceiveNext(); // always send in the same order we recieve
        }
    }
}
