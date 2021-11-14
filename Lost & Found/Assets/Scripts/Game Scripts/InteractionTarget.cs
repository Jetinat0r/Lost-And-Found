using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractionTarget : MonoBehaviour
{
    [SerializeField]
    private GameObject objectHighlight;
    //Objects that put their functions here must have a reference to this object
    [SerializeField]
    public UnityEvent checkCanInteractEvents;
    [SerializeField]
    public UnityEvent interactEvents;

    public bool canInteract = false;

    public void CheckCanInteract()
    {
        checkCanInteractEvents?.Invoke();

        ToggleInteract(canInteract);
    }

    public void ToggleInteract(bool _canInteract)
    {
        canInteract = _canInteract;
        objectHighlight.SetActive(canInteract);
    }

    public void Interact()
    {
        interactEvents?.Invoke();
    }
}
