using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField]
    private string interactableTag = "Interactable";

    private IInteractable targetedObject;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && targetedObject != null)
        {
            //Maybe something w/ lists, and you move the targeted object to the next one in the list?
            //And when you get to a new object, that shoves its way to the front?
            Debug.Log("Interact!" + targetedObject);
            targetedObject.Interact();

            targetedObject = null;
        }
    }


    //Should target the most recent object that you went into range for, probably will have to change later
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(interactableTag))
        {
            Debug.Log("In range!");
            targetedObject = collision.gameObject.GetComponent<IInteractable>();
            Debug.Log(targetedObject);

            //TODO: Remove hard coding
            QuestItemPhysical qI = collision.GetComponent<QuestItemPhysical>();
            if (qI != null)
            {
                qI.CheckIfQuestActive(QuestManager.instance.curQuests);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(interactableTag))
        {
            Debug.Log(targetedObject + "\nLeft Range!");
            if(targetedObject == collision.gameObject.GetComponent<IInteractable>())
            {
                targetedObject = null;
            }
        }
    }
}
