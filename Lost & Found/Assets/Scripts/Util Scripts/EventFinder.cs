using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public enum CallableClasses
{
    GameManager,
    AudioManager,
    SceneController,
    PlayerInventory,
    SceneInfoContainer
}

[Serializable]
public class EventFunctionParams
{
    public string name;

    public CallableClasses classToCall;

    public string functionName;

    public string[] stringParams;
    public int[] intParams;
    public float[] floatParams;
    public bool[] boolParams;
}

public class EventFinder : MonoBehaviour
{
    public static EventFinder instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //public QuestScriptableObject obj;
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.A))
    //    {
    //        obj.OnStartToInProgress();
    //    }
    //}

    //Get passed strings, ints, floats, bools in that order
    //Take only what you need, meaning extraneous is ok
    public void CallFunction(EventFunctionParams functionParams)
    {
        Type type = null;
        MethodInfo method;

        switch (functionParams.classToCall)
        {
            case (CallableClasses.GameManager):
                type = typeof(GameManager);
                method = MethodGetter(type, functionParams);

                if (method != null)
                {
                    object[] sentParams = new object[] { functionParams };
                    method.Invoke(GameManager.instance, sentParams);
                }

                break;

            case (CallableClasses.AudioManager):
                type = typeof(AudioManager);
                method = MethodGetter(type, functionParams);

                if(method != null)
                {
                    object[] sentParams = new object[] { functionParams };
                    method.Invoke(AudioManager.instance, sentParams);
                }

                break;

            case (CallableClasses.SceneController):
                type = typeof(SceneController);
                method = MethodGetter(type, functionParams);

                if(method != null)
                {
                    object[] sentParams = new object[] { functionParams };
                    method.Invoke(SceneController.instance, sentParams);
                }

                break;

            case (CallableClasses.PlayerInventory):
                type = typeof(PlayerInventory);
                method = MethodGetter(type, functionParams);

                if (method != null)
                {
                    object[] sentParams = new object[] { functionParams };
                    method.Invoke(PlayerInventory.instance, sentParams);
                }

                break;

            case (CallableClasses.SceneInfoContainer):
                type = typeof(SceneInfoContainer);
                method = MethodGetter(type, functionParams);

                if (method != null)
                {
                    object[] sentParams = new object[] { functionParams };
                    method.Invoke(SceneInfoContainer.instance, sentParams);
                }

                break;
        }
        

        if(type == null)
        {
            Debug.LogWarning("Class somehow unset for: " + functionParams.name);
            return;
        }
    }

    private MethodInfo MethodGetter(Type type, EventFunctionParams functionParams)
    {
        Type[] signature = new[] { typeof(EventFunctionParams) };

        MethodInfo method = type.GetMethod(functionParams.functionName, signature);

        if (method == null)
        {
            Debug.LogWarning("For: " + functionParams.name + "\n" + "Method not found: " + functionParams.functionName);
            return null;
        }

        return method;
    }
}
