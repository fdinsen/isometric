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

    private PlayerInput _inputActions;
    void Awake()
    {
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
        _inputActions = new PlayerInput();
        _inputActions.Movement.Enable();
        if(DialogueManager.Instance != null)
        {
            DialogueManager.Instance.DialogueStarted += () => _inputActions.Movement.Disable();
            DialogueManager.Instance.DialogueEnded += () => _inputActions.Movement.Enable();
        }
        if(_phealth != null)
        {
            _phealth.PlayerDied += (a, b) => ToggleMovement(false);
        }
        
    }

    private void FixedUpdate()
    {
        if (_view.IsMine)
        {
            Move(_inputActions.Movement.Movement.ReadValue<Vector2>());
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
            _inputActions.Movement.Enable(); return; 
        }
        _inputActions.Movement.Disable();
    }
}
