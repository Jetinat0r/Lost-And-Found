using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory instance;

    public List<QuestItemScriptableObject> curHeldItems;

    private void Awake()
    {
        //Singleton convention, allows access from anything at any time via PlayerInventory.instance
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PickupItem(QuestItemScriptableObject questItem)
    {
        foreach(QuestItemScriptableObject item in curHeldItems)
        {
            if(item == questItem)
            {
                Debug.LogWarning("Attempting to add item to inventory that is already in inventory! Returning...");
                return;
            }
        }
        curHeldItems.Add(questItem);

        //TODO: Refactor to be smaller
        //Priority: Low
        foreach (QuestScriptableObject quest in QuestManager.instance.GetActiveQuests())
        {
            if (quest.idQuestItemNames.Contains(questItem.idItemName))
            {
                bool hasAllItems = true;

                //If this loop does not break, the player has all the necessary items
                foreach(string itemName in quest.idQuestItemNames)
                {
                    bool hasItem = false;

                    foreach(QuestItemScriptableObject heldQuestItem in curHeldItems)
                    {
                        if(heldQuestItem.idItemName == itemName)
                        {
                            hasItem = true;

                            break;
                        }
                    }

                    if (!hasItem)
                    {
                        hasAllItems = false;
                        break;
                    }
                }

                if (hasAllItems)
                {
                    quest.OnInProgressToEnd();
                }
            }
        }
    }

    public void RemoveItem(QuestItemScriptableObject item)
    {
        curHeldItems.Remove(item);
    }

    public void RemoveItem(string itemId)
    {
        foreach(QuestItemScriptableObject item in curHeldItems)
        {
            if(item.idItemName == itemId)
            {
                curHeldItems.Remove(item);
                return;
            }
        }

        Debug.LogWarning("Attempted to remove item from inventory that wasn't there!");
    }

    //EventFinder Overload
    public void RemoveItem(EventFunctionParams functionParams)
    {
        RemoveItem(functionParams.stringParams[0]);
    }

    public bool CheckItem(string _itemId)
    {
        foreach(QuestItemScriptableObject item in curHeldItems)
        {
            if(_itemId == item.idItemName)
            {
                return true;
            }
        }

        return false;
    }

    public void ClearInventory()
    {
        curHeldItems = new List<QuestItemScriptableObject>();
    }
}
