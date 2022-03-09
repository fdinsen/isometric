using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public delegate void InteractionEvent();
    void Interact();
}
