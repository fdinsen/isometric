using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;


public class PlayerController : MonoBehaviour
{
    private PhotonView view;
    [SerializeField] private CharacterController _controller;
    [SerializeField] private float _speed = 5;
    [SerializeField] private Camera _camera;

    private bool facingRight = true;

    private PlayerInput _inputActions;
    void Awake()
    {
        view = GetComponent<PhotonView>();
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
        Move(_inputActions.Movement.Movement.ReadValue<Vector2>());
    }

    void Flip()
    {
        facingRight = !facingRight;

        transform.Rotate(0f, 180f, 0f);
    }

    void Move(Vector2 input)
    {
        if (input == Vector2.zero) return;
        _controller.Move(input * _speed * Time.fixedDeltaTime);

        if (input.x > 0 && !facingRight)
        {
            // ... flip the player.
            Flip();
        }
        // Otherwise if the input is moving the player left and the player is facing right...
        else if (input.x < 0 && facingRight)
        {
            // ... flip the player.
            Flip();
        }
    }
}
