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

    public void AddQuest(QuestScriptableObject _newQuest, bool _forceAdd = false)
    {
        if(_forceAdd)
        {
            curQuests.Add(_newQuest);
        }

        foreach(QuestScriptableObject _quest in curQuests)
        {
            if(_quest.idQuestName == _newQuest.idQuestName)
            {
                Debug.LogWarning("Quest (" + _newQuest.idQuestName + ") is already in curQuests! Refusing to add to list...");
                return;
            }
        }

        curQuests.Add(_newQuest);
    }

    public void RemoveQuest(QuestScriptableObject _toRemove)
    {
        bool _wasRemoved = false;
        for(int i = 0; i < curQuests.Count; i++)
        {
            if(curQuests[i].idQuestName == _toRemove.idQuestName)
            {
                curQuests.RemoveAt(i);

                _wasRemoved = true;
            }
        }

        if (!_wasRemoved)
        {
            Debug.LogWarning("Quest (" + _toRemove.idQuestName + ") was not found in curQuests!");
        }
    }

    public void ActivateQuests(List<string> _questIds)
    {
        foreach(string _id in _questIds)
        {
            bool _wasFound = false;

            foreach(QuestScriptableObject _quest in curQuests)
            {
                if(_quest.idQuestName == _id)
                {
                    _wasFound = true;
                    _quest.OnInactiveToStart();

                    break;
                }
            }

            if (!_wasFound)
            {
                Debug.LogWarning("Quest of ID (" + _id + ") was not found in curQuests!");
            }
        }
    }

    public void FailQuests(List<string> _questIds)
    {
        foreach (string _id in _questIds)
        {
            bool _wasFound = false;

            foreach (QuestScriptableObject _quest in curQuests)
            {
                if (_quest.idQuestName == _id)
                {
                    _wasFound = true;
                    _quest.OnStateToFailed();

                    break;
                }
            }

            if (!_wasFound)
            {
                Debug.LogWarning("Quest of ID (" + _id + ") was not found in curQuests!");
            }
        }
    }
}
