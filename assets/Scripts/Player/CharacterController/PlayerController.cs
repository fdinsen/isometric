using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private PhotonView view;
    [Header("Component Setup")]
    [SerializeField] private CharacterController _controller;
    [SerializeField] private Animator _anim;
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _sprite;

    [Header("Parameters")]
    [SerializeField] private float _speed = 5;

    private GameObject[] crosshair;

    private bool facingRight = true;

    private PlayerInput _inputActions;
    void Awake()
    {
        view = GetComponent<PhotonView>();
        if(_controller == null) _controller = GetComponent<CharacterController>();
        if(_anim == null) _anim = GetComponent<Animator>();
        crosshair = GameObject.FindGameObjectsWithTag("Crosshair");
        if (!view.IsMine)
        {
            _camera.gameObject.SetActive(false);
        }
        else
        {
            _inputActions = new PlayerInput();
            _inputActions.Movement.Enable();
            if(DialogueManager.Instance != null)
            {
                DialogueManager.Instance.DialogueStarted += () => _inputActions.Movement.Disable();
                DialogueManager.Instance.DialogueEnded += () => _inputActions.Movement.Enable();
            }
        }
    }

    private void FixedUpdate()
    {
        if (view.IsMine)
        {
            Move(_inputActions.Movement.Movement.ReadValue<Vector2>());
        }
    }

    void Flip(GameObject sprite)
    {
        facingRight = !facingRight;

        sprite.transform.Rotate(0f, 180f, 0f);
    }

    void Move(Vector2 input)
    {
        if(_anim != null)
            _anim.SetFloat("Speed", GetVector2Size(input));

        if (input == Vector2.zero) return;

        _controller.Move(input * _speed * Time.fixedDeltaTime);

        if (input.x > 0 && !facingRight)
        {
            // ... flip the player.
            Flip(_sprite);
        }
        // Otherwise if the input is moving the player left and the player is facing right...
        else if (input.x < 0 && facingRight)
        {
            // ... flip the player.
            Flip(_sprite);
        }
    }

    float GetVector2Size(Vector2 v)
    {
        return Mathf.Abs(v.x) + Mathf.Abs(v.y);
    }
}
