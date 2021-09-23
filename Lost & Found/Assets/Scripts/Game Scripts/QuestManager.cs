using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    public List<QuestScriptableObject> curQuests;
    //public List<QuestItemScriptableObject> activeQuestItems;

    private void Awake()
    {
        //Singleton convention, allows access from anything at any time via QuestManager.instance
        if (QuestManager.instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        foreach (QuestScriptableObject quest in curQuests)
        {
            quest.InitializeQuestState();
        }
    }

    public List<QuestScriptableObject> GetActiveQuests()
    {
        List<QuestScriptableObject> activeQuests = new List<QuestScriptableObject>();

        foreach(QuestScriptableObject quest in curQuests)
        {
            if(quest.curQuestState == QuestState.InProgress)
            {
                activeQuests.Add(quest);
            }
        }

        return activeQuests;
    }
}
