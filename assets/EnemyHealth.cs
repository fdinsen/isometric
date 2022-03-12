using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyHealth : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("Health")]
    [SerializeField] private int _initialHealth = 5;

    [Header("Recoil")]
    [SerializeField] private float _recoilforce = 1f;

    private int _health;
    public int Health { get { return _health; } private set {_health = value; } }

    public delegate void EnemyHealthEvent(int health);
    public event EnemyHealthEvent EnemyDied;

    private Rigidbody2D _rb;

    private void Start()
    {
        _health = _initialHealth;
        EnemyDied += h => StartCoroutine(Die(h));
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Debug.Log("nmy hlth " + _health);
    }

    public void DealDamage(int dmg, Vector2 hitdir)
    {
        _health -= dmg;

        float recoilforce = _recoilforce * dmg;
        if(_health <= 0)
        {
            EnemyDied.Invoke(_health);
            recoilforce *= 1f + (Mathf.Abs(_health) / _initialHealth * .01f); //adds more force the more below 0 the health went
            _rb.drag *= 0.2f; 
            Debug.Log("Recoil Force " + recoilforce);
        }
        _rb.AddForce(hitdir * recoilforce);
    }

    public IEnumerator Die(int health)
    {
        yield return new WaitForSeconds(1f);
        PhotonNetwork.Destroy(gameObject);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) // I am the owner of this object
        {
            stream.SendNext(Health); // We share the health over the network
        }
        else // Another client is the owner of this object
        {
             Health = (int)stream.ReceiveNext(); // we recieve the health over the network
        }
    }

}
