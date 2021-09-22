using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestItemPhysical : MonoBehaviour, IInteractable
{
    [SerializeField]
    private QuestItemScriptableObject questItemScriptableObject;

    private bool canPickUp = false;

    [SerializeField]
    private GameObject itemGraphics;
    //NOTE: itemHighlight should be a child of itemGraphics
    [SerializeField]
    private GameObject itemHighlight;

    public void CheckIfQuestActive(List<QuestScriptableObject> curActiveQuests)
    {
        foreach(QuestScriptableObject quest in curActiveQuests)
        {
            if(quest.curQuestState == QuestState.InProgress)
            {
                foreach (string itemName in quest.questItems)
                {
                    if (itemName == questItemScriptableObject.itemName)
                    {
                        //TODO: Activate pickup
                        canPickUp = true;

                        //ChangeItemHighlight(true);

                        return;
                    }
                }
            }
            
            
        }

        //ChangeItemHighlight(false);
        canPickUp = false;
    }

    public void ChangeItemHighlight(bool toggleOn)
    {
        itemHighlight.SetActive(toggleOn);
    }

    public void PickupItem()
    {
        Debug.Log("Add item");
        //TODO: Find player Inventory
        PlayerInventory.instance.PickupItem(questItemScriptableObject);

        Destroy(gameObject);
    }

    public void Interact()
    {
        if (canPickUp)
        {
            //TODO: pick up item
            Debug.Log("INTERACT");
            PickupItem();
        }
    }
}
