using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour, IInteractable
{
    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;
    public void Interact()
    {
        if (DialogueManager.Instance.DialogueIsPlaying) {return;}
        DialogueManager.Instance.EnterDialogueMode(inkJSON);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
