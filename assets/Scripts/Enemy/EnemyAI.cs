using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using CodeMonkey.Utils;
using System;
using Photon.Pun;

[RequireComponent(typeof(NavMeshAgent), typeof(EnemyShooting), typeof(CircleCollider2D))]
public class EnemyAI : MonoBehaviourPunCallbacks, IPunObservable
{
    private enum State
    {
        Roaming, ChaseTarget, ShootingTarget, Loitering, Dead
    }

    [Header("Layer Filters")]
    [SerializeField] [Tooltip("Layers which count as unwalkable and will result in a new roaming position to be chosen. Example: Walls")]
    private LayerMask blockedLocations;
    [SerializeField] [Tooltip("The filter used to finding targets. Example use: Set LayerMask to Player, to make the enemy chase players.")]
    private ContactFilter2D targetFilter;

    [Header("AI Variables")]
    [SerializeField] [Tooltip("The distance within the player has to be for the enemy to spot them.")]
    private float sightRange = 10f;
    [SerializeField] private float attackRange = 3f;
    [SerializeField] [Tooltip("The distance which the player being chased has to be outside for the enemy to lose sight of them.")]
    private float loseSightDistance = 20f;
    [SerializeField] [Tooltip("The minimum range for standing still between roaming moves, in seconds.")]
    private float loiterTimeMin = .5f;
    [SerializeField] [Tooltip("The maximum range for standing still between roaming moves, in seconds.")]
    private float loiterTimeMax = 1.5f;

    [Header("Stuck Detection")]
    [SerializeField] [Tooltip("If the enemy doesn't move more than this distance for the given amount of frames, it counts as it being stuck.")] 
    private float _stuckDetectionDistance = 0.01f;
    [SerializeField] [Tooltip("The amount of frames the character has to be unmoving for for it to count as being stuck.")]
    private int _stuckDetectionFrameLimit = 30;
    
    private int _stuckDetectionFrames = 0;
    private Vector3 _stuckPositionCache;

    private State _state;
    private NavMeshAgent _agent;
    private EnemyShooting _shooter;
    private EnemyHealth _health;
    private Animator _anim;
    private Vector3 _startingPos;
    private Vector3 _roamPos;



    private GameObject _currentTarget;
    private PhotonView _targetView;
    private List<Collider2D> _targets;

    private float _timeUntilStopLoitering;

    private void Awake()
    {
        _state = State.Roaming;

    }
    // Start is called before the first frame update
    void Start()
    {
        _stuckPositionCache = transform.position;
        _anim = GetComponentInChildren<Animator>();
        _shooter = GetComponent<EnemyShooting>();
        _health = GetComponent<EnemyHealth>();
        _targets = new List<Collider2D>();
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateUpAxis = false;
        _agent.updateRotation = false;
        _startingPos = transform.position;
        _roamPos = GetRoamingPosition();
        targetFilter.useLayerMask = true;
        if (_health) _health.EnemyDied += _ => _state = State.Dead;

        _agent.SetDestination(_roamPos);
        transform.rotation = Quaternion.Euler(0, 0, transform.rotation.z);
    }

    private void FixedUpdate()
    {
        
        if (ViewHasOwnership())
        {
            switch (_state)
            {
                default:
                    break;
                case State.Roaming:
                    Roam();
                    break;
                case State.ChaseTarget:
                    ChaseTarget();
                    break;
                case State.ShootingTarget:
                    _shooter.Shoot(GetPlayerDir(), () => _state = State.ChaseTarget);
                    break;
                case State.Loitering:
                    Loiter();
                    break;
                case State.Dead:
                    _agent.isStopped = true;
                    break;
            }
        }
    }

    private void Roam()
    {
        _anim.SetFloat("Speed", GetVector2Size(_agent.velocity));
        float reachedPositionDistance = 2f;
        if (Vector3.Distance(transform.position, _roamPos) < reachedPositionDistance)
        {
            //Reached roam position
            _timeUntilStopLoitering = Time.time + UnityEngine.Random.Range(loiterTimeMin, loiterTimeMax);
            _state = State.Loitering;
        }
        if (VectorHasNotChangedWithinTime(transform.position))
        {
            // immediately stop loitering. Effectively just finds a new roaming position.
            _timeUntilStopLoitering = Time.time;
            _state = State.Loitering;
        }
        FindTarget();
    }

    private void Loiter()
    {
        if (Time.time > _timeUntilStopLoitering)
        {
            _roamPos = GetRoamingPosition();
            _agent.SetDestination(_roamPos);
            _state = State.Roaming;
        }
    }

    private void ChaseTarget()
    {
        
        if (_currentTarget == null)
        {
            _state = State.Roaming;
            return;
        }
        _agent.SetDestination(_currentTarget.transform.position);
        _anim.SetFloat("Speed", GetVector2Size(_agent.velocity));
        if (Vector3.Distance(transform.position, _currentTarget.transform.position) < attackRange)
        {
            //Target within attack range
            _agent.isStopped = true;
            _state = State.ShootingTarget;
        }
        else
        {
            _agent.isStopped = false;
            if (Vector3.Distance(transform.position, _currentTarget.transform.position) > loseSightDistance)
            {
                // Too far, stop chasing
                _currentTarget = null;
                _startingPos = transform.position;
                _state = State.Roaming;
            }
        }
    }

    public Vector3 GetRoamingPosition()
    {
        var loc = _startingPos + UtilsClass.GetRandomDir() * UnityEngine.Random.Range(3f, 10f);
        if (Physics2D.OverlapCircle(loc, 2f, blockedLocations))
        {
            loc = GetRoamingPosition();
        }
        return loc;
    }

    public void FindTarget()
    {
       
        Physics2D.OverlapCircle(transform.position, sightRange, targetFilter, _targets);
        if (_targets.Count != 0)
        {
            _currentTarget = _targets[0].gameObject;
            _targetView = _currentTarget.GetComponent<PhotonView>();
            _state = State.ChaseTarget;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRange);

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) // I am the owner of this object
        {
            stream.SendNext(_startingPos); // We share the health over the network
            stream.SendNext(_roamPos); // always send in the same order we recieve
            stream.SendNext(_state);
        }
        else // Another client is the owner of this object
        {
            _startingPos = (Vector3)stream.ReceiveNext(); // we recieve the health over the network
            _roamPos = (Vector3)stream.ReceiveNext(); // always send in the same order we recieve
            _state = (State)stream.ReceiveNext();
        }
    }

    private bool ViewHasOwnership()
    {
        return PhotonNetwork.IsMasterClient;
    }

    private Vector2 GetPlayerDir()
    {
        var dir = (transform.position - _currentTarget.transform.position).normalized;
        return -dir;
    }

    private float GetVector2Size(Vector2 v)
    {
        return Mathf.Abs(v.x) + Mathf.Abs(v.y);
    }

    private bool VectorHasNotChangedWithinTime(Vector3 currentVector)
    {
        if (!VectorsAreWithinRange(_stuckPositionCache, currentVector, _stuckDetectionDistance))
        {
            _stuckPositionCache = transform.position;
            return false;
        }
        if (_stuckDetectionFrames >= _stuckDetectionFrameLimit)
        {
            return true;
        }            
        _stuckDetectionFrames++;
        return false;
    }

    private bool VectorsAreWithinRange(Vector3 oldVector, Vector3 newVector, float range)
    {
        return Vector3.Distance(oldVector, newVector) < range;
    }
}