using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WorldNode
{
    public Rect rect;
    public string title;
    public bool isDragged;
    public bool isSelected;

    public bool createdMenu;
    public bool hasNewConnection;

    public GUIStyle style;
    public GUIStyle selectedStyle;
    public GUIStyle connectorStyle;

    //onConnectionDraw:
    //goto entrance and destination nodes
    //find which connection index it is
    //use inPoint or outPoint transforms
    public List<WorldNodeConnector> incomingConnections;
    public List<Rect> inPoints;

    public List<WorldNodeConnector> outgoingConnections;
    public List<Rect> outPoints;

    public WorldNode(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle selectedNodeStyle, GUIStyle _connectorStyle)
    {
        rect = new Rect(position.x, position.y, width, height);
        style = nodeStyle;
        selectedStyle = selectedNodeStyle;
        connectorStyle = _connectorStyle;
    }

    public void Drag(Vector2 delta)
    {
        rect.position += delta;

        //Don't need to check inPoints or outPoints bc they are tied to the other lists
        if(incomingConnections != null)
        {
            for (int i = 0; i < inPoints.Count; i++)
            {
                Rect tempRect = inPoints[i];
                tempRect.position += delta;
                inPoints[i] = tempRect;
            }
        }
        
        if(outgoingConnections != null)
        {
            for (int i = 0; i < outPoints.Count; i++)
            {
                Rect tempRect = outPoints[i];
                tempRect.position += delta;
                outPoints[i] = tempRect;
            }
        }
    }

    public void Draw()
    {
        DrawConnectors();

        GUI.Box(rect, title, style);
    }

    public void DrawConnectors()
    {
        if(outgoingConnections != null)
        {
            foreach (WorldNodeConnector connector in outgoingConnections)
            {
                connector.Draw();
            }
        }
    }

    private bool ProcessConnectorEvents(Event e)
    {
        if(outgoingConnections != null)
        {
            bool connectorEvents = false;

            foreach (WorldNodeConnector connector in outgoingConnections)
            {
                if (connector.ProcessEvents(e))
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
        bool connectorEvents = ProcessConnectorEvents(e);
        createdMenu = false;

        switch (e.type)
        {
            case (EventType.MouseDown):
                if(e.button == 0)
                {
                    if (rect.Contains(e.mousePosition))
                    {
                        isDragged = true;
                        GUI.changed = true;

                        isSelected = true;

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

                    createdMenu = true;
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
        }

        return false;
    }

    private void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Create connection"), false, () => CreateConnection(mousePosition));
        genericMenu.AddItem(new GUIContent("Destroy node"), false, () => OnClickDestroyNode());
        genericMenu.ShowAsContext();
    }

    private void CreateConnection(Vector2 startPos)
    {
        if(outgoingConnections == null)
        {
            outgoingConnections = new List<WorldNodeConnector>();
            outPoints = new List<Rect>();
        }

        outPoints.Add(new Rect(startPos, new Vector2(0, 0)));
        outgoingConnections.Add(new WorldNodeConnector(this, startPos, connectorStyle));
    }

    private void OnClickDestroyNode()
    {
        Debug.Log("Oh no, you destroyed the node!");
    }

    //Used for incoming connections (destination of connectors)
    public Vector2 GetInPoint(WorldNodeConnector connector)
    {
        int index = -1;

        if (incomingConnections == null)
        {
            Debug.LogWarning("No in points on node: " + this.title);
            return Vector2.zero;
        }

        for (int i = 0; i < incomingConnections.Count; i++)
        {
            if(incomingConnections[i] == connector)
            {
                index = i;
                break;
            }
        }
        //WorldNodeConnector point = Array.Find(incomingConnections, connection => connection == connector);

        if(index == -1)
        {
            Debug.LogWarning("Connector: " + connector + " does not go to this node: " + this.title);
            return Vector2.zero;
        }

        return new Vector2(inPoints[index].x, inPoints[index].y);
    }

    //Used for outgoing connections (entrance of connectors)
    public Vector2 GetOutPoint(WorldNodeConnector connector)
    {
        int index = -1;

        if(outgoingConnections == null)
        {
            Debug.LogWarning("No out points on node: " + this.title);
            return Vector2.zero;
        }

        for (int i = 0; i < outgoingConnections.Count; i++)
        {
            if (outgoingConnections[i] == connector)
            {
                index = i;
                break;
            }
        }
        //WorldNodeConnector point = Array.Find(incomingConnections, connection => connection == connector);

        if (index == -1)
        {
            Debug.LogWarning("Connector: " + connector + " does not come from this node: " + this);
            return Vector2.zero;
        }

        return new Vector2(outPoints[index].x, outPoints[index].y);
    }

    public void RemoveIncomingConnection(WorldNodeConnector connector)
    {
        int index = -1;
        for(int i = 0; i < incomingConnections.Count; i++)
        {
            if(incomingConnections[i] == connector)
            {
                index = i;
            }
        }

        if(index == -1)
        {
            Debug.LogWarning("Connector not found in incoming connections, something went wrong!");
            return;
        }

        incomingConnections.RemoveAt(index);
        inPoints.RemoveAt(index);
    }

    public void RemoveOutgoingConnection(WorldNodeConnector connector)
    {
        int index = -1;
        for (int i = 0; i < outgoingConnections.Count; i++)
        {
            if (outgoingConnections[i] == connector)
            {
                index = i;
            }
        }

        if (index == -1)
        {
            Debug.LogWarning("Connector not found in outgoing connections, something went wrong!");
            return;
        }

        outgoingConnections.RemoveAt(index);
        outPoints.RemoveAt(index);
    }
}
