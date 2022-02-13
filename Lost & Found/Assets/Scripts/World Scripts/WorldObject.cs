using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWorld", menuName = "ScriptableObjects/World")]
public class WorldObject : ScriptableObject
{
    #region User Variables
    public string title;
    #endregion

    //nodeCount used to ensure no two nodes have the same filename
    [HideInInspector]
    public int nodeCount = 0;
    public List<WorldNode> nodes;

    // These should be accessible if the user wants to reset the window, scale is hidden bc it doesn't work
    [HideInInspector]
    public float scale = 1f;
    public Vector2 offset = Vector2.zero;

    //if a new connection exists, returns the index
    //else returns no
    public int CheckNewConnections()
    {
        if (nodes != null)
        {
            for(int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].hasNewConnection)
                {
                    return i;
                }
            }
        }

        return -1;
    }

    public void DestroyNode(WorldNode node)
    {
        //Called from a node, so nodes is never null
        //Unless it is, in which case - uh oh
        nodes.Remove(node);
    }

    public WorldNode GetNode(string _nodeName)
    {
        foreach(WorldNode _node in nodes)
        {
            if(_node.title == _nodeName)
            {
                return _node;
            }
        }

        Debug.LogWarning("Node (" + _nodeName + ") not found! Returning first node of the world...");
        return nodes[0];
    }
}
