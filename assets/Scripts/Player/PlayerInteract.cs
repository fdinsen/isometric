using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private PlayerInput _playerInput;

    [SerializeField] private LayerMask interactableLayers = new LayerMask();

    // Start is called before the first frame update
    void Start()
    {
        _playerInput = new PlayerInput();
        _playerInput.Movement.Enable();

        _playerInput.Movement.Interact.performed += ctx => Interact();
        DialogueManager.Instance.DialogueStarted += () => { _playerInput.Movement.Disable(); };
        DialogueManager.Instance.DialogueEnded += () => { _playerInput.Movement.Enable(); };
    
    }
    void OnDisable()
    {
        _playerInput.Movement.Disable();
    }

    // Update is called once per frame
    void Interact()
    {
        var interacted = Physics.OverlapSphere(transform.position, 10f, interactableLayers);
        foreach(Collider col in interacted)
        {
            //Debug.Log(col.gameObject.name);
            col.GetComponent<IInteractable>().Interact();
        }
    }
}
