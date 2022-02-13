using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public TimeBlock curTime;
    public List<GamePeriod> timeSlots;
    //public List<QuestScriptableObject> curQuests;

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
}
