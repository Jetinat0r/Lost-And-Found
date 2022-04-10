using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Journal : MonoBehaviour
{
    public enum InventoryPanel
    {
        QuestPanel = 0,
        ItemPanel,
        MapPanel,
        SettingsPanel
    }


    [SerializeField]
    private GameObject journalPanel;

    [Space(10)]

    [SerializeField]
    private GameObject questPanel;
    [SerializeField]
    private GameObject questDisplayArea;
    [SerializeField]
    private JournalQuestDisplay questDisplayPrefab;

    [Space(10)]

    [SerializeField]
    private GameObject itemPanel;
    [SerializeField]
    private GameObject itemDisplayArea;
    [SerializeField]
    private JournalItemDisplay itemDisplayPrefab;

    [Space(10)]

    [SerializeField]
    private GameObject mapPanel;
    [SerializeField]
    private GameObject mapImage;

    [Space(10)]

    [SerializeField]
    private GameObject settingsPanel;


    [HideInInspector]
    public bool isOpen = false;
    private InventoryPanel lastOpenPanel = InventoryPanel.QuestPanel;


    public void OpenJournal()
    {
        //Set base active
        journalPanel.SetActive(true);

        //Set last open panel active
        SetPanel(lastOpenPanel);

        isOpen = true;
    }

    public void SetPanel(InventoryPanel panel)
    {
        DeactivateAllPanels();

        switch (panel)
        {
            case (InventoryPanel.QuestPanel):
                lastOpenPanel = InventoryPanel.QuestPanel;

                questPanel.SetActive(true);
                DisplayQuests();
                break;

            case (InventoryPanel.ItemPanel):
                lastOpenPanel = InventoryPanel.ItemPanel;

                itemPanel.SetActive(true);
                DisplayItems();
                break;

            case (InventoryPanel.MapPanel):
                lastOpenPanel = InventoryPanel.MapPanel;

                mapPanel.SetActive(true);
                DisplayMap();
                break;

            case (InventoryPanel.SettingsPanel):
                lastOpenPanel = InventoryPanel.SettingsPanel;

                settingsPanel.SetActive(true);
                break;

            default:
                Debug.LogWarning("Invalid panel! Activating Quest Panel...");
                lastOpenPanel = InventoryPanel.QuestPanel;

                questPanel.SetActive(true);
                DisplayQuests();
                break;
        }
    }

    //Here so that the buttons can use it
    public void SetPanel(string panelType)
    {
        InventoryPanel panel;

        switch (panelType)
        {
            case ("QuestPanel"):
                panel = InventoryPanel.QuestPanel;
                break;

            case ("ItemPanel"):
                panel = InventoryPanel.ItemPanel;
                break;

            case ("MapPanel"):
                panel = InventoryPanel.MapPanel;
                break;

            case ("SettingsPanel"):
                panel = InventoryPanel.SettingsPanel;
                break;

            default:
                Debug.LogWarning("Invalid panel type (" + panelType + "), using QuestPanel!");
                panel = InventoryPanel.QuestPanel;
                break;
        }

        SetPanel(panel);
    }

    private void DeactivateAllPanels()
    {
        questPanel.SetActive(false);
        itemPanel.SetActive(false);
        mapPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    private void DisplayQuests()
    {
        List<QuestScriptableObject> _questsToDisplay = GetQuestsToDisplay();

        //Remove Previously Displayed Quests
        foreach(Transform _t in questDisplayArea.transform)
        {
            Util.DestroyRecursively(_t.gameObject);
        }

        //Display Quests
        foreach(QuestScriptableObject _quest in _questsToDisplay)
        {
            //Make a new obj
            JournalQuestDisplay _newDisplay = Instantiate(questDisplayPrefab, itemDisplayArea.transform);

            //Set properties
            _newDisplay.SetData(_quest.displayQuestName, _quest.displayNpcName, _quest.displayQuestDescription);
        }
    }

    private List<QuestScriptableObject> GetQuestsToDisplay()
    {
        List<QuestScriptableObject> _quests = new List<QuestScriptableObject>();

        //Throw out all quests not in InProgress, End, or ?Completed/Failed
        //Also throw out non-displayed quests
        foreach(QuestInfo _info in GameManager.instance.curQuestInfos)
        {
            QuestScriptableObject _quest = _info.quest;

            //States for which the quest should NOT be displayed
            if(!_quest.isDisplayed ||
                _quest.curQuestState == QuestState.Inactive ||
                _quest.curQuestState == QuestState.Start ||
                _quest.curQuestState == QuestState.Completed ||
                _quest.curQuestState == QuestState.Failed)
            {
                continue;
            }

            _quests.Add(_quest);
        }

        return _quests;
    }

    private void DisplayItems()
    {
        List<QuestItemScriptableObject> _itemsToDisplay = GetItemsToDisplay();

        //Remove Previously Displayed Quests
        foreach (Transform _t in itemDisplayArea.transform)
        {
            Util.DestroyRecursively(_t.gameObject);
        }

        //Display Items
        foreach (QuestItemScriptableObject _item in _itemsToDisplay)
        {
            //Make a new obj
            JournalItemDisplay _newDisplay = Instantiate(itemDisplayPrefab, itemDisplayArea.transform);

            //Set properties
            _newDisplay.SetData(_item.displayItemName, _item.itemDescription);
        }
    }

    private List<QuestItemScriptableObject> GetItemsToDisplay()
    {
        List<QuestItemScriptableObject> _items = new List<QuestItemScriptableObject>();

        //Throw out non-displayed items
        foreach(QuestItemScriptableObject _item in PlayerInventory.instance.curHeldItems)
        {
            //States for which the item should NOT be displayed
            if (!_item.isDisplayed)
            {
                continue;
            }

            _items.Add(_item);
        }

        return _items;
    }

    private void DisplayMap()
    {
        //TODO: Check a bool or an item
        mapImage.SetActive(true);
    }

    public void CloseJournal()
    {
        journalPanel.SetActive(false);
    }
}
