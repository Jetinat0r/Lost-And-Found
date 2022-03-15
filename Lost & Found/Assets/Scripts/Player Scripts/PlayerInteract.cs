using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField]
    private string interactableTag = "Interactable";
    [SerializeField]
    private Animator animator;

    private List<InteractionTarget> targetedObjects = new List<InteractionTarget>();
    //isBusy will be used for things like minigames or dialogue, maybe even transferring through doors
    //Should be handled by the thing that hinders player movement
    public bool isBusy = false;

    // Update is called once per frame
    void Update()
    {
        if (!isBusy && Input.GetKeyDown(KeyCode.E) && targetedObjects.Count != 0)
        {
            //Maybe something w/ lists, and you move the targeted object to the next one in the list?
            //And when you get to a new object, that shoves its way to the front?
            //Debug.Log("Interact!" + targetedObject);
            targetedObjects[0].Interact();

            animator.Play("Player_Grab");

            //TODO: alter for isBusy
            targetedObjects.RemoveAt(0);
        }
    }

    private void HighlightInteractable()
    {
        //this is here so that when you get in range, use an interactable, etc it will highlight the next one in the list

        //Step 1: foreach target in targetedObjects - set highlight to false
        foreach (InteractionTarget target in targetedObjects)
        {
            target.SetHighlight(false);
        }
        //Step 2: highlight element 0 of targeted Objects
        targetedObjects[0].SetHighlight(true);
    }

    public void EnableInteract()
    {
        isBusy = false;
    }

    public void DisableInteract()
    {
        isBusy = true;
    }

    //Should target the most recent object that you went into range for, probably will have to change later
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(interactableTag))
        {
            // Debug.Log("In range!");
            InteractionTarget newTarget = collision.gameObject.GetComponent<InteractionTarget>();
            
            // Debug.Log(targetedObject);

            if (!newTarget.itCanInteract)
            {
                //If the targeted object can not be interacted with, fail
                return;
            }

            targetedObjects.Add(newTarget);

            HighlightInteractable();

            //--- NOTE: Really not sure what this is or where I wanted to move it to, so I'm leaving it jic

            //TODO: Remove hard coding, move to 
            //QuestItemPhysical qI = collision.GetComponent<QuestItemPhysical>();
            //if (qI != null)
            //{
            //    qI.CheckIfQuestActive(QuestManager.instance.curQuests);
            //}
            
            //--- :P
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(interactableTag))
        {
            // Debug.Log(targetedObject + "\nLeft Range!");
            InteractionTarget target = collision.gameObject.GetComponent<InteractionTarget>();
            if (targetedObjects.Contains(target))
            {
                target.SetHighlight(false);
                targetedObjects.Remove(target);
            }
        }
    }
}
