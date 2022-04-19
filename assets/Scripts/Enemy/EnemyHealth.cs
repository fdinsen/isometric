using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyHealth : MonoBehaviourPunCallbacks, IPunObservable, IHurtable
{
    [Header("Health")]
    [SerializeField] private int _initialHealth = 5;

    [Header("Recoil")]
    [SerializeField] private float _recoilforce = 1f;

    private int _health;
    public int Health { get { return _health; } private set {_health = value; } }

    private bool _hasBeenHitByActivePlayer = false;

    public delegate void EnemyHealthEvent(EnemyHealthEventArgs e);
    public event EnemyHealthEvent ThisEnemyDied;
    public static event EnemyHealthEvent AnEnemyDied;
    public static event EnemyHealthEvent OnAnEnemyDamaged;

    private Rigidbody2D _rb;

    private void Start()
    {
        _health = _initialHealth;
        ThisEnemyDied += e => StartCoroutine(Die(e.health));
        _rb = GetComponent<Rigidbody2D>();
    }

    public void DealDamage(int dmg)
    {
        _health -= dmg;
        _hasBeenHitByActivePlayer = true;
        OnAnEnemyDamaged?.Invoke(SetArgs());

        if (_health <= 0)
        {
            ThisEnemyDied.Invoke(SetArgs());
        }
    }

    public void DealDamage(int dmg, Vector2 hitdir)
    {
        _health -= dmg;
        _hasBeenHitByActivePlayer = true;
        OnAnEnemyDamaged?.Invoke(SetArgs(hitdir));

        float recoilforce = _recoilforce * dmg;
        if(_health <= 0)
        {
            ThisEnemyDied.Invoke(SetArgs(hitdir));
            recoilforce *= 1f + (Mathf.Abs(_health) / _initialHealth * .01f); //adds more force the more below 0 the health went
            _rb.drag *= 0.2f; 
            //Debug.Log("Recoil Force " + recoilforce);
        }
        _rb.AddForce(hitdir * recoilforce);
    }

    public IEnumerator Die(int health)
    {
        AnEnemyDied?.Invoke(SetArgs());
        yield return new WaitForSeconds(1f);
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
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

    private EnemyHealthEventArgs SetArgs()
    {
        return new EnemyHealthEventArgs(_health, transform.position, _hasBeenHitByActivePlayer);
    }

    private EnemyHealthEventArgs SetArgs(Vector3 hitdir)
    {
        return new EnemyHealthEventArgs(_health, transform.position, _hasBeenHitByActivePlayer, hitdir);
    }


    public class EnemyHealthEventArgs
    {
        public int health;
        public Vector3 pos;
        public Vector3 hitdir = Vector3.zero;
        public bool hasBeenHitByActivePlayer;

        public EnemyHealthEventArgs(int health, Vector3 pos, bool hit)
        {
            this.health = health;
            this.pos = pos;
            this.hasBeenHitByActivePlayer = hit;
        }
        public EnemyHealthEventArgs(int health, Vector3 pos, bool hit, Vector3 hitdir)
        {
            this.health = health;
            this.pos = pos;
            this.hasBeenHitByActivePlayer = hit;
            this.hitdir = hitdir;
        }

    }
}
