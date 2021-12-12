using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class WorldNodeConnector : ScriptableObject
{
    #region User Variables
    public string title = "";
    public List<string> reqItems = new List<string>();
    public bool isLocked = false;
    public string lockedText = "";
    #endregion

    //NOTE: Many public variables with [HideInInspector] above them are supposed to be private, but needed to be public so that they could be modified
    //      through EditorUtility.SetDirty()


    //private string path;
    [HideInInspector]
    public float clickWidth = 10f;

    [HideInInspector]
    public Rect rect;
    //If the endpoint is set yet
    private bool isSet = false;
    [HideInInspector]
    public bool isSelected = false;

    [HideInInspector]
    public GUIStyle style;
    [HideInInspector]
    public GUIStyle selectedStyle;

    public WorldNode entranceNode;
    public WorldNode destinationNode;
    [HideInInspector]
    public Vector2 tempEndPoint;

    [HideInInspector]
    public float arrowSize = 16f;

    //private float drawnHeight = 10f;

    public void SetupWorldNodeConnector(WorldNode parentNode, Vector2 mousePosition, GUIStyle connectorStyle, GUIStyle selectedConnectorStyle, string path)
    {
        entranceNode = parentNode;
        style = connectorStyle;
        selectedStyle = selectedConnectorStyle;
        tempEndPoint = mousePosition;
    }

    public bool ProcessEvents(Event e)
    {
        //if(e.type == EventType.MouseDown)
        //{
        //    isSelected = false;
        //}

        EditorUtility.SetDirty(this);

        //if (e.type == EventType.Used)
        //{
        //    isSelected = false;
        //    return true;
        //}

        switch (e.type)
        {
            //case (EventType.KeyDown):
            //    if (e.keyCode == KeyCode.Escape)
            //    {
            //        GUI.changed = true;
            //        CancelConnector();
            //    }
            //    break;

            case (EventType.MouseDown):
                if (e.button == 0)
                {
                    isSelected = false;


                    Vector2 startPoint = entranceNode.GetOutPoint(this);

                    Vector2 endPoint;
                    if (destinationNode == null)
                    {
                        endPoint = tempEndPoint;
                    }
                    else
                    {
                        endPoint = destinationNode.GetInPoint(this);
                    }

                    #region Check if mouse is in a reasonable position

                    //(clickWidth) shows up bc if the x's or y's are the same the cursor has to be EXACTLY on that point to work
                    if (startPoint.x < endPoint.x)
                    {
                        if (e.mousePosition.x < startPoint.x - (clickWidth) || e.mousePosition.x > endPoint.x + (clickWidth))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (e.mousePosition.x < endPoint.x - (clickWidth) || e.mousePosition.x > startPoint.x + (clickWidth))
                        {
                            return false;
                        }
                    }

                    if (startPoint.y < endPoint.y)
                    {
                        if (e.mousePosition.y < startPoint.y - (clickWidth) || e.mousePosition.y > endPoint.y + (clickWidth))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (e.mousePosition.y < endPoint.y - (clickWidth) || e.mousePosition.y > startPoint.y + (clickWidth))
                        {
                            return false;
                        }
                    }
                    #endregion

                    //Determine if the mouse is between the two points
                    float mouseDist = PointDistance(startPoint, e.mousePosition) + PointDistance(e.mousePosition, endPoint);
                    float lineDist = PointDistance(startPoint, endPoint);

                    //Debug.Log(mouseDist);
                    //Debug.Log(lineDist);
                    if (mouseDist < lineDist + clickWidth)
                    {
                        GUI.changed = true;

                        isSelected = true;
                        AssetDatabase.OpenAsset(this);

                        //e.Use();
                        return true;
                    }
                    //AttemptPlace(e.mousePosition);
                }
                else if (isSet && e.button == 1)
                {
                    isSelected = false;


                    Vector2 startPoint = entranceNode.GetOutPoint(this);

                    Vector2 endPoint;
                    if (destinationNode == null)
                    {
                        endPoint = tempEndPoint;
                    }
                    else
                    {
                        endPoint = destinationNode.GetInPoint(this);
                    }

                    #region Check if mouse is in a reasonable position

                    //(clickWidth / 2) shows up bc if the x's or y's are the same the cursor has to be EXACTLY on that point to work

                    if (startPoint.x < endPoint.x)
                    {
                        if (e.mousePosition.x < startPoint.x - (clickWidth / 2) || e.mousePosition.x > endPoint.x + (clickWidth / 2))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (e.mousePosition.x < endPoint.x - (clickWidth / 2) || e.mousePosition.x > startPoint.x + (clickWidth / 2))
                        {
                            return false;
                        }
                    }

                    if (startPoint.y < endPoint.y)
                    {
                        if (e.mousePosition.y < startPoint.y - (clickWidth / 2) || e.mousePosition.y > endPoint.y + (clickWidth / 2))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (e.mousePosition.y < endPoint.y - (clickWidth / 2) || e.mousePosition.y > startPoint.y + (clickWidth / 2))
                        {
                            return false;
                        }
                    }
                    #endregion

                    //Determine if the mouse is between the two points
                    float mouseDist = PointDistance(startPoint, e.mousePosition) + PointDistance(e.mousePosition, endPoint);
                    float lineDist = PointDistance(startPoint, endPoint);

                    
                    if (mouseDist < lineDist + clickWidth)
                    {
                        GUI.changed = true;

                        isSelected = true;
                        ProcessContextMenu(e.mousePosition);

                        e.Use();
                        return true;
                    }
                    //else if (e.button == 1)
                    //{
                    //    if (isSet && rect.Contains(e.mousePosition))
                    //    {
                    //        //ProcessContextMenu(e.mousePosition);
                    //    }
                    //    else
                    //    {
                    //        //GUI.changed = true;
                    //        //CancelConnector();
                    //    }
                    //}
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

            case (EventType.Used):
                isSelected = false;

                break;
        }

        return false;
    }

    private float PointDistance(Vector2 pointA, Vector2 pointB)
    {
        return Mathf.Sqrt(Mathf.Pow((pointA.x - pointB.x), 2) + Mathf.Pow((pointA.y - pointB.y), 2));
    }

    public void CancelConnector()
    {
        //Potentially needed to tell the above things that we are no longer holding a connector
        //If not needed, just do "DestroyConnector" where this shows up

        Debug.Log("Connector Canceled");
        entranceNode.hasNewConnection = false;

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

            //Only true if the connector has been placed, so this is safe
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(this));
        }

        DestroyImmediate(this, true);
    }

    public void AttemptPlace(WorldNode node, Vector2 mousePos)
    {
        if(node != entranceNode)
        {
            destinationNode = node;
            destinationNode.AddIncomingConnection(this, mousePos);

            entranceNode.AddConnectorFile(this);
            isSet = true;
            //entranceNode.hasNewConnection = false;
        }
        else
        {
            Debug.LogWarning("Destination node can't be the same as entrance node!");
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


        Color drawColor;
        if (isSelected)
        {
            drawColor = Color.blue;
        }
        else
        {
            drawColor = Color.white;
        }

        Handles.DrawBezier(startPoint, endPoint, startPoint, endPoint, drawColor, null, 2.5f);
        
        
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
        //rect = new Rect(startPoint, Vector2.zero)
        //{
        //    xMin = startPoint.x,
        //    //xMax = startPoint.x + drawnHeight,
        //    xMax = startPoint.x + Mathf.Abs(endPoint.x - startPoint.x),
        //    yMin = startPoint.y,
        //    //yMax = startPoint.y + drawnHeight
        //    yMax = startPoint.y + Mathf.Abs(endPoint.y - startPoint.y)
        //};


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
        GUI.DrawTexture(new Rect(arrowStartX, arrowStartY - (arrowSize / 2), arrowSize, arrowSize), connectionArrow, ScaleMode.ScaleToFit, true, 1f, drawColor, 0f, 0f);
        GUIUtility.RotateAroundPivot(-theta, new Vector2(arrowStartX, arrowStartY));
    }

    private void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();

        genericMenu.AddItem(new GUIContent("Destroy Connection"), false, () => DestroyConnector());
        genericMenu.ShowAsContext();
    }
}
