using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class WorldNodeConnector
{
    public Rect rect;
    //If the endpoint is set yet
    private bool isSet = false;

    public GUIStyle style;

    public WorldNode entranceNode;
    public WorldNode destinationNode;
    public Vector2 tempEndPoint;

    private float arrowSize = 16f;

    private float drawnHeight = 10f;

    public WorldNodeConnector(WorldNode parentNode, Vector2 mousePosition, GUIStyle nodeStyle)
    {
        entranceNode = parentNode;
        style = nodeStyle;
        tempEndPoint = mousePosition;
    }

    public bool ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case (EventType.KeyDown):
                if (e.keyCode == KeyCode.Escape)
                {
                    CancelConnector();
                }
                break;

            case (EventType.MouseDown):
                if (e.button == 0)
                {
                    //AttemptPlace(e.mousePosition);
                }
                else if (e.button == 1)
                {
                    if (isSet && rect.Contains(e.mousePosition))
                    {
                        //ProcessContextMenu(e.mousePosition);
                    }
                    else
                    {
                        CancelConnector();
                    }
                }
                break;

            case (EventType.MouseUp):
                
                break;

            case (EventType.MouseDrag):
                if (!isSet)
                {
                    //Debug.Log("heh");
                    //tempEndPoint = e.mousePosition;
                    //return true;
                }
                break;

            case (EventType.MouseMove):
                if (!isSet)
                {
                    //Debug.Log("Hey!");
                    tempEndPoint = e.mousePosition;

                    GUI.changed = true;
                    return true;
                }
                break;
        }

        return false;
    }

    public void CancelConnector()
    {
        //Potentially needed to tell the above things that we are no longer holding a connector
        //If not needed, just do "DestroyConnector" where this shows up

        Debug.Log("Connector Canceled");

        DestroyConnector();
    }

    public void DestroyConnector()
    {
        if (entranceNode != null)
        {
            entranceNode.RemoveOutgoingConnection(this);
        }

        if(destinationNode != null)
        {
            destinationNode.RemoveIncomingConnection(this);
        }

        //Destroy(this);

        //TODO: figure out how to destroy this
        //      oh god deleting files
    }

    public bool AttemptPlace(WorldNode node)
    {
        if(node != entranceNode)
        {
            destinationNode = node;
            return true;
        }
        else
        {
            Debug.LogWarning("Destination node can't be the same as entrance node!");
            return false;
        }
    }

    public void Draw()
    {
        
        
        //Get start and end points
        Vector2 startPoint = entranceNode.GetOutPoint(this);

        Vector2 endPoint;
        if(destinationNode == null)
        {
            endPoint = tempEndPoint;
        }
        else
        {
            endPoint = destinationNode.GetInPoint(this);
        }

        
        Handles.DrawBezier(startPoint, endPoint, startPoint, endPoint, Color.white, null, 2.5f);

        
        //Calc length of arrow
        float length = Mathf.Sqrt(Mathf.Pow(endPoint.x - startPoint.x, 2) + Mathf.Pow(endPoint.y - startPoint.y, 2));

        //Avoids a div by zero error
        if(length == 0)
        {
            length = 0.001f;
        }

        //float arrowStartX = endPoint.x - startPoint.x + (length / 2);
        //float arrowStartY = endPoint.y - startPoint.y + (length / 2);
        float arrowStartX = (startPoint.x + endPoint.x) / 2;
        float arrowStartY = (startPoint.y + endPoint.y) / 2;
        
        //Make a new rect for clicking
        rect = new Rect(startPoint, Vector2.zero)
        {
            xMin = startPoint.x,
            xMax = startPoint.x + drawnHeight,
            yMin = startPoint.y,
            yMax = startPoint.y + drawnHeight
        };


        //Calc angle of rotation
        float theta = Mathf.Asin((endPoint.y - startPoint.y) / length);
        theta *= Mathf.Rad2Deg;

        
        //Adjust theta for mouse pos
        if (endPoint.x < startPoint.x)
        {
            if(endPoint.y < startPoint.y)
            {
                theta = -90 + (-90 - theta);
            }
            else
            {
                theta = 90 + (90 - theta);
            }
        }

        Texture2D connectionArrow = EditorGUIUtility.Load("Node_Arrow.png") as Texture2D;

        //Some jank ass rotation code bc the only way to rotate shit is to rotate the entire friggin window matrix
        GUIUtility.RotateAroundPivot(theta, new Vector2(arrowStartX, arrowStartY));
        //GUI.Box(rect, GUIContent.none, style);
        GUI.DrawTexture(new Rect(arrowStartX, arrowStartY - (arrowSize / 2), arrowSize, arrowSize), connectionArrow);
        GUIUtility.RotateAroundPivot(-theta, new Vector2(arrowStartX, arrowStartY));
    }
}
