using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.SceneManagement;

public class WorldNode : ScriptableObject
{
    [System.Serializable]
    public class NodeSceneReference
    {
        public TimeBlock timeBlock;

        public string sceneTitle = "";
    }

    #region User Variables
    public string title = "new_world_node";
    public List<NodeSceneReference> sceneList = new List<NodeSceneReference>();
    #endregion

    //NOTE: Many public variables with [HideInInspector] above them are supposed to be private, but needed to be public so that they could be modified
    //      through EditorUtility.SetDirty()

    [HideInInspector]
    public WorldObject world;
    [HideInInspector]
    public string path;

    [HideInInspector]
    public Rect rect;
    
    [HideInInspector]
    public bool isDragged;
    [HideInInspector]
    public bool isSelected;
    [HideInInspector]
    public bool hasNewConnection;

#if (UNITY_EDITOR)
    [HideInInspector]
    public GUIStyle style;
    [HideInInspector]
    public GUIStyle selectedStyle;
    [HideInInspector]
    public GUIStyle connectorStyle;
    [HideInInspector]
    public GUIStyle selectedConnectorStyle;
#endif

    [System.Serializable]
    public class NodeConnection
    {
        public WorldNodeConnector connector;
        public Rect connectionPoint;
    }

    //onConnectionDraw:
    //goto entrance and destination nodes
    //find which connection index it is
    //use inPoint or outPoint transforms
    //public List<WorldNodeConnector> incomingConnections;
    //public List<Rect> inPoints;

    public List<NodeConnection> incomingConnections;

    //public List<WorldNodeConnector> outgoingConnections;
    //public List<Rect> outPoints;

    public List<NodeConnection> outgoingConnections;

#if (UNITY_EDITOR)
    #region GUI Functions
    //Called upon Instantiation
    public void SetupNode(WorldObject _world, Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle selectedNodeStyle, GUIStyle _connectorStyle, GUIStyle _selectedConnectorStyle, string _path)
    {
        world = _world;
        rect = new Rect(position.x, position.y, width, height);
        style = nodeStyle;
        selectedStyle = selectedNodeStyle;
        connectorStyle = _connectorStyle;
        selectedConnectorStyle = _selectedConnectorStyle;
        path = _path;

        incomingConnections = new List<NodeConnection>();
        outgoingConnections = new List<NodeConnection>();

        EditorUtility.SetDirty(this);
    }

    public void Drag(Vector2 delta)
    {
        rect.position += delta;

        //Don't need to check inPoints or outPoints bc they are tied to the other lists
        if(incomingConnections != null)
        {
            for (int i = 0; i < incomingConnections.Count; i++)
            {
                incomingConnections[i].connectionPoint.position += delta;
            }
        }
        
        if(outgoingConnections != null)
        {
            for (int i = 0; i < outgoingConnections.Count; i++)
            {
                outgoingConnections[i].connectionPoint.position += delta;
            }
        }
    }

    public void Draw()
    {
        //DrawConnectors();

        GUIStyle nodeStyle;

        if (isSelected)
        {
            //Debug.Log(selectedStyle.normal.background);
            nodeStyle = selectedStyle;
        }
        else
        {
            nodeStyle = style;
        }

        GUI.Box(rect, title, nodeStyle);
    }

    public void DrawConnectors()
    {
        if(outgoingConnections != null)
        {
            foreach (NodeConnection nodeConnection in outgoingConnections)
            {
                nodeConnection.connector.Draw();
            }
        }
    }

    private bool ProcessConnectorEvents(Event e)
    {
        if (outgoingConnections != null)
        {
            bool connectorEvents = false;

            //WARNING: .ToArray() IS SUPER HACKY AND APPARENTLY SHOULD NEVER
            //         BE USED, BUT AVOIDS AN ERROR SO I DON'T CARE
            //NOTE: using classes may have changed this error, but I'm too scared to check
            foreach (NodeConnection nodeConnection in outgoingConnections.ToArray())
            {
                if (nodeConnection.connector.ProcessEvents(e))
                {
                    connectorEvents = true;
                }
            }

            return connectorEvents;
        }

        return false;
    }

    public bool ProcessEvents(Event e)
    {
        ProcessConnectorEvents(e);

        //if (e.type == EventType.Used)
        //{
        //    return true;
        //}

        EditorUtility.SetDirty(this);

        switch (e.type)
        {
            case (EventType.MouseDown):
                if(e.button == 0)
                {
                    isSelected = false;

                    if (rect.Contains(e.mousePosition))
                    {
                        if(world.CheckNewConnections() != -1)
                        {
                            TellConnectorPlace(this, e.mousePosition);
                        }
                        else
                        {
                            isDragged = true;
                            GUI.changed = true;

                            isSelected = true;
                            AssetDatabase.OpenAsset(this);
                        }
                    }
                    else
                    {
                        GUI.changed = true;

                        isSelected = false;
                    }
                }
                else if(e.button == 1 && rect.Contains(e.mousePosition))
                {
                    ProcessContextMenu(e.mousePosition);

                    e.Use();
                }
                break;

            case (EventType.MouseUp):
                isDragged = false;
                break;

            case (EventType.MouseDrag):
                if(isDragged && e.button == 0)
                {
                    Drag(e.delta);
                    e.Use();
                    return true;
                }
                break;

            case (EventType.Used):
                isSelected = false;
                break;
        }

        return false;
    }

    private void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();

        if (world.CheckNewConnections() != -1)
        {
            genericMenu.AddItem(new GUIContent("Place Connection"), false, () => TellConnectorPlace(this, mousePosition));
        }

        genericMenu.AddItem(new GUIContent("Create Connection"), false, () => CreateConnection(mousePosition));
        genericMenu.AddItem(new GUIContent("Destroy Node"), false, () => OnClickDestroyNode());
        genericMenu.ShowAsContext();
    }

    private void CreateConnection(Vector2 startPos)
    {
        if(outgoingConnections == null)
        {
            outgoingConnections = new List<NodeConnection>();
            //outPoints = new List<Rect>();
        }

        //TODO: properly deleting


        NodeConnection nodeConnection = new NodeConnection();
        nodeConnection.connectionPoint = new Rect(startPos, Vector2.zero);

        WorldNodeConnector connector = ScriptableObject.CreateInstance<WorldNodeConnector>();
        nodeConnection.connector = connector;

        connector.SetupWorldNodeConnector(this, startPos, connectorStyle, selectedConnectorStyle, path + "/" + "Connectors" + "/");
        outgoingConnections.Add(nodeConnection);
        
        //outgoingConnections.Add(new WorldNodeConnector(this, startPos, connectorStyle));

        hasNewConnection = true;
    }

    public void CancelConnection()
    {
        hasNewConnection = false;

        NodeConnection connection = outgoingConnections[outgoingConnections.Count - 1];

        connection.connector.DestroyConnector();
        outgoingConnections.Remove(connection);
        //outgoingConnections[outgoingConnections.Count - 1].connector.DestroyConnector();
        //outgoingConnections.RemoveAt(outgoingConnections.Count - 1);
    }

    //node should always be "this" but whatever
    public void TellConnectorPlace(WorldNode node, Vector2 mousePos)
    {
        //Super spaghetti hell, help me
        WorldNode startNode = world.nodes[world.CheckNewConnections()];
        startNode.outgoingConnections[startNode.outgoingConnections.Count - 1].connector.AttemptPlace(node, mousePos);
    }

    public void AddIncomingConnection(WorldNodeConnector nodeConnector, Vector2 mousePos)
    {
        if (incomingConnections == null)
        {
            incomingConnections = new List<NodeConnection>();
        }

        NodeConnection nodeConnection = new NodeConnection();
        nodeConnection.connectionPoint = new Rect(mousePos, Vector2.zero);

        nodeConnection.connector = nodeConnector;

        incomingConnections.Add(nodeConnection);
    }

    public void AddConnectorFile(WorldNodeConnector nodeConnector)
    {
        hasNewConnection = false;

        

        //TODO: take destination and origin titles and add to file output
        if (!AssetDatabase.IsValidFolder(path + "/" + "Connectors"))
        {
            AssetDatabase.CreateFolder(path, "Connectors");
        }

        AssetDatabase.CreateAsset(nodeConnector, path + "/" + "Connectors" + "/" + nodeConnector.entranceNode.outgoingConnections.Count + "_" + nodeConnector.entranceNode.title + "_TO_" + nodeConnector.destinationNode.title + ".asset");
    }

    private void OnClickDestroyNode()
    {
        for(int i = 0; i < outgoingConnections.Count; i++)
        {
            outgoingConnections[i].connector.DestroyConnector();
        }

        for(int i = 0; i < incomingConnections.Count; i++)
        {
            incomingConnections[i].connector.DestroyConnector();
        }

        world.DestroyNode(this);

        string assetPath = AssetDatabase.GetAssetPath(this);
        string folderPath = Path.GetDirectoryName(assetPath);

        AssetDatabase.DeleteAsset(assetPath);
        AssetDatabase.DeleteAsset(folderPath);

        DestroyImmediate(this);
    }

    //Used for incoming connections (destination of connectors)
    public Vector2 GetInPoint(WorldNodeConnector connector)
    {
        if (incomingConnections == null)
        {
            Debug.LogWarning("No in points on node: " + this.title);
            return Vector2.zero;
        }

        for (int i = 0; i < incomingConnections.Count; i++)
        {
            if(incomingConnections[i].connector == connector)
            {
                
                return new Vector2(incomingConnections[i].connectionPoint.x, incomingConnections[i].connectionPoint.y);
            }
        }
        //WorldNodeConnector point = Array.Find(incomingConnections, connection => connection == connector);

        Debug.LogWarning("Connector: " + connector + " does not go to this node: " + this.title);
        return Vector2.zero;
    }

    //Used for outgoing connections (entrance of connectors)
    public Vector2 GetOutPoint(WorldNodeConnector connector)
    {
        if(outgoingConnections == null)
        {
            Debug.LogWarning("No out points on node: " + this.title);
            return Vector2.zero;
        }

        for (int i = 0; i < outgoingConnections.Count; i++)
        {
            if (outgoingConnections[i].connector == connector)
            {
                return new Vector2(outgoingConnections[i].connectionPoint.x, outgoingConnections[i].connectionPoint.y);
            }
        }
        //WorldNodeConnector point = Array.Find(incomingConnections, connection => connection == connector);

        Debug.LogWarning("Connector: " + connector + " does not come from this node: " + this);
        return Vector2.zero;
    }

    public void RemoveIncomingConnection(WorldNodeConnector connector)
    {
        for(int i = 0; i < incomingConnections.Count; i++)
        {
            if(incomingConnections[i].connector == connector)
            {
                incomingConnections.RemoveAt(i);
                return;
            }
        }

        Debug.LogWarning("Connector not found in incoming connections, something went wrong!");
        return;
    }

    public void RemoveOutgoingConnection(WorldNodeConnector connector)
    {
        for (int i = 0; i < outgoingConnections.Count; i++)
        {
            if (outgoingConnections[i].connector == connector)
            {
                outgoingConnections.RemoveAt(i);
                return;
            }
        }

        Debug.LogWarning("Connector not found in outgoing connections, something went wrong!");
        return;
    }
    #endregion
#endif

    public WorldNode GetConnectedNode(string _connectionTitle)
    {
        foreach (NodeConnection _connectionStruct in outgoingConnections)
        {
            if(_connectionStruct.connector.title == _connectionTitle)
            {
                return _connectionStruct.connector.destinationNode;
            }
        }

        Debug.LogWarning("No connector with title " + _connectionTitle + " exists, so no node could be found! Returning current node...");
        return this;
    }

    public string GetSceneTitle(GamePeriod _period)
    {
        foreach(NodeSceneReference _sceneReference in sceneList)
        {
            if (_sceneReference.timeBlock.IsEqual(_period.timeBlock))
            {
                return _sceneReference.sceneTitle;
            }
        }

        //This only runs if a scene is not found
        Debug.LogWarning("No scene found in node " + this.title + " for period: " + "\n" + _period.ToString());
        return sceneList[0].sceneTitle;
    }

    public WorldNodeConnector GetConnectorFromTitle(string _connectionTitle)
    {
        for(int i = 0; i < outgoingConnections.Count; i++)
        {
            WorldNodeConnector _curConnector = outgoingConnections[i].connector;
            if (_curConnector.title == _connectionTitle)
            {
                return _curConnector;
            }
        }

        Debug.LogWarning("Connection (" + _connectionTitle + ") does not go out from node (" + title + "), ERRORS/ISSUES INCOMING!");
        return outgoingConnections[0].connector;
    }
}
