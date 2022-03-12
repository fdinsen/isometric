using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    
    private PlayerInput _playerInput;

    [SerializeField] private float interactDistance = 1f;
    [SerializeField] private LayerMask interactableLayers = new LayerMask();

    // Start is called before the first frame update
    void Start()
    {
        _playerInput = new PlayerInput();
        _playerInput.Movement.Enable();

        _playerInput.Movement.Interact.performed += ctx => Interact();
        DialogueManager.Instance.DialogueStarted += () => { _playerInput.Movement.Disable(); };
        DialogueManager.Instance.DialogueEnded += () => { _playerInput.Movement.Enable(); };

        TryGetComponent<PlayerHealth>(out var pHealth);
        if (pHealth != null)
        {
            pHealth.PlayerDied += (a, b) => _playerInput.Movement.Disable();
        }
    }
    void OnDisable()
    {
        _playerInput.Movement.Disable();
    }

    // Update is called once per frame
    void Interact()
    {
        Debug.Log("Hi");
        var interacted = Physics.OverlapSphere(transform.position, interactDistance, interactableLayers);
        foreach(Collider col in interacted)
        {
            Debug.Log(col.gameObject.name);
            col.GetComponent<IInteractable>().Interact();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}
