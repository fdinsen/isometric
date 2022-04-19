using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using Cinemachine;

//[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    #region Class Variables
    private PhotonView _view;
    [Header("Component Setup")]
    [SerializeField] private CinemachineVirtualCamera _camera;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private Collider2D _hitboxCollider;
    [SerializeField] private Animator _spriteAnim;
    [SerializeField] private PlayerHealth _phealth;

    [Header("Movement")]
    [SerializeField] [Tooltip("The movement speed of the Player.")] private float _speed = 5;
    private bool _facingRight = true;

    [Header("Dodging")]
    [SerializeField] [Tooltip("The force of the dodge. Multiples of movement speed")] 
    private float _dodgeForce = 1.5f;
    [SerializeField] [Tooltip("The time in seconds which the dodge will take. Affects the distance dodged.")] 
    private float _dodgeTime = .5f;
    private bool _dodging = false;

    private GameObject[] _crosshair;
    #endregion

    #region Public Attributes
    public PlayerInput PlayerInput { get; private set; }
    #endregion

    #region Unity Setup
    void Awake()
    {
        //All players execute following
        _view = GetComponent<PhotonView>();
        if(_rb == null) _rb = GetComponent<Rigidbody2D>();
        if(_spriteAnim == null) _spriteAnim = GetComponentInChildren<Animator>();
        if (_phealth == null) _phealth = GetComponent<PlayerHealth>();

        _crosshair = GameObject.FindGameObjectsWithTag("Crosshair");
        _camera.gameObject.transform.parent = null;
        CameraManager.Instance.AddVirtualCamera(_camera);
        if (!_view.IsMine)
        {
            gameObject.tag = "OtherPlayer";
            _camera.Priority = 1;
            //_camera.gameObject.SetActive(false);
            return;
        }

        //Only owner executes following
        PlayerInput = new PlayerInput();
        PlayerInput.Movement.Enable();
        PlayerInput.Movement.Dodge.performed += ctx => Dodge();

        if(_phealth != null)
        {
            _phealth.PlayerDied += (a, b) => ToggleMovement(false);
        }
        PlayerInput.MenuControls.Exit.performed += ctx => ExitGame();
    }

    private void FixedUpdate()
    {
        if (_view.IsMine && !_dodging)
        {
            Move(PlayerInput.Movement.Movement.ReadValue<Vector2>());
        }
    }

    private void OnDestroy()
    {
        CameraManager.Instance.RemoveVirtualCamera(_camera);
    }
    #endregion

    #region Movement
    void Move(Vector2 input)
    {
        if(_spriteAnim != null)
            _spriteAnim.SetFloat("Speed", GetVector2Size(input));

        if (input == Vector2.zero) return;

        var moveBy = new Vector3(input.x, input.y, 0);
        transform.position += _speed * Time.fixedDeltaTime * moveBy;

        if (input.x > 0 && !_facingRight)
        {
            // ... flip the player.
            Flip(_spriteAnim.gameObject);
        }
        // Otherwise if the input is moving the player left and the player is facing right...
        else if (input.x < 0 && _facingRight)
        {
            // ... flip the player.
            Flip(_spriteAnim.gameObject);
        }
    }

    void Flip(GameObject sprite)
    {
        _facingRight = !_facingRight;

        sprite.transform.Rotate(0f, 180f, 0);
    }
    #endregion

    #region Dodge
    void Dodge()
    {
        var dir = PlayerInput.Movement.Movement.ReadValue<Vector2>().normalized;
        if(!_dodging) StartCoroutine(PerformDodge(dir * _dodgeForce * _speed));
    }

    private IEnumerator PerformDodge(Vector2 dodgeBy)
    {
        _dodging = true;
        if(_hitboxCollider != null) _hitboxCollider.enabled = false;
        _rb.velocity += dodgeBy;
        yield return new WaitForSeconds(_dodgeTime);
        _dodging = false;
        if (_hitboxCollider != null) _hitboxCollider.enabled = true;
        _rb.velocity = Vector2.zero;
    }
    #endregion

    #region Animations
    public void PlayAttackAnimation()
    {
        _spriteAnim.SetTrigger("Attack");
    }

    public void PlayHurtAnimation()
    {
        _spriteAnim.SetTrigger("Hurt");
    }

    public void PlayDeathAnimation()
    {
        _spriteAnim.SetBool("Dead", true);
    }
    #endregion

    #region Helper Functions
    private void ToggleMovement(bool doEnable)
    {
        if (doEnable)
        {
            PlayerInput.Movement.Enable(); return;
        }
        PlayerInput.Movement.Disable();
    }

    float GetVector2Size(Vector2 v)
    {
        return Mathf.Abs(v.x) + Mathf.Abs(v.y);
    }
    //public void ReactivateCamera()
    //{
    //    _camera.SetActive(true);
    //    //return _camera;
    //}

    public CinemachineVirtualCamera GetCamera()
    {
        return _camera;
    }
    #endregion

    #region Testing
    void ExitGame()
    {
        Debug.Log("Closing game..");
        PhotonNetwork.Disconnect();
        Application.Quit();
    }
    #endregion
}
