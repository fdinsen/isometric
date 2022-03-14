using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

//[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private PhotonView _view;
    [Header("Component Setup")]
    [SerializeField] private GameObject _camera;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private Animator _spriteAnim;
    [SerializeField] private PlayerHealth _phealth;

    [Header("Parameters")]
    [SerializeField] private float _speed = 5;

    private GameObject[] _crosshair;

    private bool _facingRight = true;

    public PlayerInput PlayerInput { get; private set; }
    void Awake()
    {
        //All players execute following
        _view = GetComponent<PhotonView>();
        if(_rb == null) _rb = GetComponent<Rigidbody2D>();
        if(_spriteAnim == null) _spriteAnim = GetComponentInChildren<Animator>();
        if (_phealth == null) _phealth = GetComponent<PlayerHealth>();

        _crosshair = GameObject.FindGameObjectsWithTag("Crosshair");
        if (!_view.IsMine)
        {
            gameObject.tag = "OtherPlayer";
            _camera.gameObject.SetActive(false);
            return;
        }

        //Only owner executes following
        PlayerInput = new PlayerInput();
        PlayerInput.Movement.Enable();

        if(_phealth != null)
        {
            _phealth.PlayerDied += (a, b) => ToggleMovement(false);
        }
        PlayerInput.MenuControls.Exit.performed += ctx => ExitGame();
    }

    private void FixedUpdate()
    {
        if (_view.IsMine)
        {
            Move(PlayerInput.Movement.Movement.ReadValue<Vector2>());
        }
    }

    void Flip(GameObject sprite)
    {
        _facingRight = !_facingRight;

        sprite.transform.Rotate(0f,180f,0);
    }

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

    float GetVector2Size(Vector2 v)
    {
        return Mathf.Abs(v.x) + Mathf.Abs(v.y);
    }

    private void ToggleMovement(bool doEnable)
    {
        if (doEnable) 
        { 
            PlayerInput.Movement.Enable(); return; 
        }
        PlayerInput.Movement.Disable();
    }

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

    void ExitGame()
    {
        Debug.Log("Closing game..");
        PhotonNetwork.Disconnect();
        Application.Quit();
    }
}
