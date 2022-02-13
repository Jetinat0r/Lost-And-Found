using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractionTarget : MonoBehaviour/*, IInteractable*/
{
    //The "IT" before variable names are to separate them from the variables of the classes that inherit this

    [SerializeField]
    private GameObject itObjectHighlight;
    [SerializeField]
    public UnityEvent itInteractEvents;

    //itCanInteract is controlled by classes that inherit it
    public bool itCanInteract = true;

    public virtual void DetermineState()
    {
        SetInteract(itCanInteract);
    }

    public void SetInteract(bool _canInteract)
    {
        itCanInteract = _canInteract;
    }

    //public void EnableHighlight()
    //{
    //    ITobjectHighlight.SetActive(true);
    //}

    public void SetHighlight(bool _isHighlighted)
    {
        itObjectHighlight.SetActive(_isHighlighted);
    }

    public void Interact()
    {
        itInteractEvents?.Invoke();
    }
}
