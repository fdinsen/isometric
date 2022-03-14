using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float interactDistance = 1f;
    [SerializeField] private LayerMask interactableLayers = new LayerMask();

    private PlayerController _player;
    private PhotonView _view;

    // Start is called before the first frame update
    void Start()
    {
        _view = GetComponent<PhotonView>();
        if (_view.IsMine)
        {
            _player = GetComponent<PlayerController>();
            _player.PlayerInput.Movement.Enable();

            _player.PlayerInput.Movement.Interact.performed += ctx => Interact();
            DialogueManager.Instance.DialogueStarted += () => { _player.PlayerInput.Movement.Disable(); };
            DialogueManager.Instance.DialogueEnded += () => { _player.PlayerInput.Movement.Enable(); };
        }
    }

    void Interact()
    {
        var interacted = Physics.OverlapSphere(transform.position, interactDistance, interactableLayers);
        foreach(Collider col in interacted)
        {
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
