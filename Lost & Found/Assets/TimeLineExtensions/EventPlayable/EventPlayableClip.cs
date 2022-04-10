using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class EventPlayableClip : PlayableAsset, ITimelineClipAsset
{
    public EventPlayableBehaviour template = new EventPlayableBehaviour ();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<EventPlayableBehaviour>.Create (graph, template);
        EventPlayableBehaviour clone = playable.GetBehaviour ();
        return playable;
    }
}
