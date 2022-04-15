using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

//Used to hold extraneous info about the current scene that would be assigned during design
public class SceneInfoContainer : MonoBehaviour
{
    public static SceneInfoContainer instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Already a SceneInfoContainer in scene, something's gone wrong! Destroying...");
            Destroy(gameObject);
        }
    }

    [System.Serializable]
    public struct ConnectionInfo
    {
        public string connectionTitle;
        public Vector3 connectionPosition;
    }

    [System.Serializable]
    public struct CutsceneInfo
    {
        public string cutsceneTitle;
        public PlayableDirector timeline;
    }

    //Pretty much used for doors
    [SerializeField]
    private List<ConnectionInfo> connections;
    [SerializeField]
    private List<CutsceneInfo> cutscenes;

    //Gives spawn positions of doors essentially
    public Vector3 GetConnectionPosition(string _connectionTitle)
    {
        if(connections.Count == 0)
        {
            Debug.LogWarning("No connections found in scene! Returning 0 vector...");
            return Vector3.zero;
        }

        for(int i = 0; i < connections.Count; i++)
        {
            if(connections[i].connectionTitle == _connectionTitle)
            {
                return connections[i].connectionPosition;
            }
        }

        Debug.LogWarning("No connections with title (" + _connectionTitle + ") found in connection list! Returning first connection...");
        return connections[0].connectionPosition;
    }

    //Used for when no connection is used to go between nodes
    public Vector3 GetConnectionPosition()
    {
        if (connections.Count == 0)
        {
            Debug.LogWarning("No connections found in scene! Returning 0 vector...");
            return Vector3.zero;
        }

        return connections[0].connectionPosition;
    }

    public void StartCutscene(string _cutsceneTitle)
    {
        foreach(CutsceneInfo cutsceneInfo in cutscenes)
        {
            if(cutsceneInfo.cutsceneTitle == _cutsceneTitle)
            {
                cutsceneInfo.timeline.Play();
                return;
            }
        }

        Debug.Log("Cutscene of title (" + _cutsceneTitle + ") not found in SceneInfoContainer!");
    }
}
