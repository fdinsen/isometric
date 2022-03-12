using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerHealth : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] int maxHealth = 100;
    public int Health { get; private set; }
    public int MaxHealth { get { return maxHealth; } private set { maxHealth = value; } }
    public delegate void HealthEvent(int health, int maxHealth);
    public event HealthEvent HealthChanged;
    public event HealthEvent PlayerDied;

    private Animator _anim;
    //private PhotonView view;

    void Update()
    {
        //Debug.Log(gameObject.tag + " health: " + Health);
    }
    void Start()
    {
        HealthChanged += (a, b) => { }; // do nothing action
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
        Debug.Log(gameObject.tag + " has died and is going to disappear in 1s!");
        yield return new WaitForSeconds(1f);
        Debug.Log(gameObject.tag + " has died and is going to disappear now!");
        //gameObject.SetActive(false);
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
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
