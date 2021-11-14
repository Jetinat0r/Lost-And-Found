using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField]
    private string interactableTag = "Interactable";
    [SerializeField]
    private Animator animator;

    private InteractionTarget targetedObject;
    //isBusy will be used for things like minigames or dialogue, maybe even transferring through doors
    //Should be handled by the thing that hinders player movement
    public bool isBusy = false;

    // Update is called once per frame
    void Update()
    {
        if (!isBusy && Input.GetKeyDown(KeyCode.E) && targetedObject != null)
        {
            //Maybe something w/ lists, and you move the targeted object to the next one in the list?
            //And when you get to a new object, that shoves its way to the front?
            //Debug.Log("Interact!" + targetedObject);
            targetedObject.Interact();

            animator.Play("Player_Grab");

            //TODO: alter for isBusy
            targetedObject = null;
        }
    }


    //Should target the most recent object that you went into range for, probably will have to change later
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(interactableTag))
        {
            // Debug.Log("In range!");
            targetedObject = collision.gameObject.GetComponent<InteractionTarget>();
            // Debug.Log(targetedObject);

            targetedObject.CheckCanInteract();
            if (!targetedObject.canInteract)
            {
                //If the targeted object can not be interacted with, fail
                targetedObject = null;
            }

            //TODO: Remove hard coding, move to 
            //QuestItemPhysical qI = collision.GetComponent<QuestItemPhysical>();
            //if (qI != null)
            //{
            //    qI.CheckIfQuestActive(QuestManager.instance.curQuests);
            //}
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(interactableTag))
        {
            // Debug.Log(targetedObject + "\nLeft Range!");
            if(targetedObject == collision.gameObject.GetComponent<InteractionTarget>())
            {
                targetedObject = null;
            }
        }
    }
}
