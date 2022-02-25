using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public TimeBlock curTime;
    public List<GamePeriod> timeSlots;

    [HideInInspector]
    public List<QuestInfo> curQuestInfos = new List<QuestInfo>();
    [HideInInspector]
    public List<FillerNpcInfo> curFillerNpcInfos = new List<FillerNpcInfo>();
    [HideInInspector]
    public List<PhysicalQuestItemInfo> curPhysicalQuestItemInfos = new List<PhysicalQuestItemInfo>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //TODO: Move to GameManager or something
    //Why did i leave this. This IS GameManager
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Test(QuestScriptableObject.FunctionParams functionParams)
    {
        Debug.Log("Test: " + functionParams.name);
    }

    // funcParams is ALWAYS string[], int[], float[], bool[]
    //public void Test(string[] strings, int[] ints, float[] floats, bool[] bools)
    //{
    //    Debug.Log("Test Succ!" + ints[0]);
    //}
    
    public GamePeriod GetCurrentPeriod()
    {
        foreach (GamePeriod period in timeSlots)
        {
            if (period.timeBlock.IsEqual(curTime))
            {
                return period;
            }
        }

        Debug.LogWarning("Time Period not found! Returning first period...");
        return timeSlots[0];
    }

    //Loads the next available period, starting with [0]
    public void ChangeGamePeriod()
    {
        GamePeriod periodToLoad = null;

        //Looks for the Game Period corresponding to the current Time Block,
        //and then gets the next one from the list.
        //If the time block is the last one, we should be using a different behavior
        for (int i = 0; i < timeSlots.Count - 1; i++)
        {
            if (curTime.Equals(timeSlots[i].timeBlock))
            {
                periodToLoad = timeSlots[i + 1];
                break;
            }
        }

        if (periodToLoad == null)
        {
            Debug.Log("No Game Periods found with curTime, or on the last Game Period. Loading first period...");
            periodToLoad = timeSlots[0];
        }

        LoadPeriod(periodToLoad);
    }

    //Loads the period with the given TimeBlock
    //If the time block is not found, loads the first period
    public void ChangeGamePeriod(TimeBlock timeBlock)
    {
        GamePeriod periodToLoad = null;

        for(int i = 0; i < timeSlots.Count; i++)
        {
            if (timeBlock.Equals(timeSlots[i].timeBlock))
            {
                periodToLoad = timeSlots[i];
                break;
            }
        }

        if(periodToLoad == null)
        {
            periodToLoad = timeSlots[0];
            Debug.LogWarning("No Game Period found with time: (Day: " + timeBlock.day + "; Time: " + timeBlock.time + ")! Loading first Game Period...");
        }

        LoadPeriod(periodToLoad);
    }

    private void LoadPeriod(GamePeriod periodToLoad)
    {
        //TODO: Display Time Block on UI to indicate change
        curTime = periodToLoad.timeBlock;

        #region Load GamePeriod Infos into "cur's"
        foreach (QuestInfo info in periodToLoad.questInfos)
        {
            //TODO: Add some checks to ensure the same info isn't added twice anywhere
            info.quest.InitializeQuestState();
            curQuestInfos.Add(info);
        }

        foreach (FillerNpcInfo info in periodToLoad.fillerNpcInfos)
        {
            //TODO: Add some checks to ensure the same info isn't added twice anywhere
            curFillerNpcInfos.Add(info);
        }

        foreach(PhysicalQuestItemInfo info in periodToLoad.physicalQuestItemInfos)
        {
            //TODO: Add some checks to ensure the same info isn't added twice anywhere
            curPhysicalQuestItemInfos.Add(info);
        }
        #endregion
        SceneController.instance.GotoNode(periodToLoad.startingNodeId);
    }
}
