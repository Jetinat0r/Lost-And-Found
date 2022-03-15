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
    [SerializeField]
    public UnityEvent itPostInteractEvents;

    [Tooltip("Usually true, only set to false if something like a cutscene happens directly after interacting")]
    public bool itCanMoveAfter = true;
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

    public virtual void Interact()
    {
        //TODO: Stop player movement
        GameManager.instance.DisablePlayerInput();
        itInteractEvents?.Invoke();
    }

    public virtual void EndInteract()
    {
        //TODO: Reinstate player movement
        itPostInteractEvents?.Invoke();
    }
}
