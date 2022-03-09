using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractionHandler : MonoBehaviour, IInteractable
{
    [SerializeField] private string targetScene;

    public void Interact()
    {
        //SceneHandler.Instance.LoadScene(targetScene);
    }
}
