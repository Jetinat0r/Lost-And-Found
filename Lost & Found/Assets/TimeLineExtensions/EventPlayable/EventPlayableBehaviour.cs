using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class EventPlayableBehaviour : PlayableBehaviour
{
    public EventFunctionParams functionToCall = new EventFunctionParams();
    public bool hasPlayedYet = false;
    public override void OnPlayableCreate (Playable playable)
    {
        hasPlayedYet = false;
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (hasPlayedYet == false)
        {
#if (UNITY_EDITOR)
            Debug.Log("There may be a null reference error about EventPlayableBehavior.ProcessFrame under this, that's because EventFinder does not exist in the scene, don't worry about it.");
#endif
            EventFinder.instance.CallFunction(functionToCall);
            hasPlayedYet = true;
        }
    }
}
