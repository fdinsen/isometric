using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueHandler : MonoBehaviour
{
    [SerializeField] GameObject _dialogueBox;
    [SerializeField] TMP_Text _text;

    //public delegate void DialogueEvent(Dialogue dialogue);
    //public static event DialogueEvent DoDialogue;

    //private PlayerMovement _movement;
    private PlayerInput _input;
    private Queue<string> _currentDialogueQueue;

    // Start is called before the first frame update
    //void Awake()
    //{
    //    //_movement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    //    _input = new PlayerInput();

    //    _input.Dialogue.Continue.performed += ctx => NextDialogue(_currentDialogueQueue);

    //    _dialogueBox.SetActive(false);
    //    DoDialogue += HandleDialogue;
    //}

    //void HandleDialogue(Dialogue dialogue)
    //{
    //    //SwitchInput(dialogue != null);
    //    _dialogueBox.SetActive(dialogue != null);
    //    if(dialogue != null)
    //    {
    //        _currentDialogueQueue = new Queue<string>(dialogue.content);
    //        NextDialogue(_currentDialogueQueue);
    //    }
    //}

    //void SwitchInput(bool beginDialogue)
    //{
    //    PlayerMovement.InvokeToggleMovement(!beginDialogue);
    //    //I wish I knew a prettier way...
    //    if (beginDialogue)
    //    {
    //        _input.Dialogue.Enable();
    //    }else
    //    {
    //        _input.Dialogue.Disable();
    //    }
    //}

    //void NextDialogue(Queue<string> diaQueue)
    //{
    //    if(diaQueue.Count == 0)
    //    {
    //        HandleDialogue(null);
    //        return;
    //    }
    //    _text.text = diaQueue.Dequeue();
    //}

    //public static void InvokeDialogue(Dialogue dialogue)
    //{
    //    DoDialogue.Invoke(dialogue);
    //}
}
