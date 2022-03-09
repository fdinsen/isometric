using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Character_Controller : MonoBehaviour
{
    private PhotonView view;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _speed = 5;
    [SerializeField] private float _turnSpeed = 1;
    [SerializeField] private Camera _camera;
    private Vector3 _input;
    private PlayerInput _inputActions;


    void Awake()
    {
        view = GetComponent<PhotonView>();
        if (!view.IsMine)
        {
            _camera.gameObject.SetActive(false);
        }else
        {
            _inputActions = new PlayerInput();
            _inputActions.Movement.Enable();
            DialogueManager.Instance.DialogueStarted += () => _inputActions.Movement.Disable();
            DialogueManager.Instance.DialogueEnded += () => _inputActions.Movement.Enable();
        }
    }

    private void Update()
    {
       if (view.IsMine)
        {
            GatherInput();
            Look();
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void GatherInput()
    {
        var val = _inputActions.Movement.Movement.ReadValue<Vector2>();
        _input = new Vector3(val.x, 0, val.y);
    }

    private void Look()
    {
        if (_input == Vector3.zero) return;

        var rot = Quaternion.LookRotation(_input.ToIso(), Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, _turnSpeed * Time.deltaTime);
    }

    private void Move()
    {
        _rb.MovePosition(transform.position + transform.forward * _input.normalized.magnitude * _speed * Time.deltaTime);
    }
}

public static class Helpers
{
    private static Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
    public static Vector3 ToIso(this Vector3 input) => _isoMatrix.MultiplyPoint3x4(input);
}