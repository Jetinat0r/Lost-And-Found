using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WorldNodeEditor : EditorWindow
{
    private List<WorldNode> nodes;

    private GUIStyle nodeStyle;
    private GUIStyle connectorStyle;

    public float scale = 1f;

    //private Vector2 offset = Vector2.zero;
    private Vector2 drag = Vector2.zero;

    //TODO: onRightClick && noNodesClicked:
    //move all nodes by delta
    //do not move connections

    [MenuItem("Custom Tools/World Editor")]
    private static void OpenWindow()
    {
        WorldNodeEditor window = GetWindow<WorldNodeEditor>();
        window.titleContent = new GUIContent("World Editor");
    }

    private void OnEnable()
    {
        wantsMouseMove = true;

        nodeStyle = new GUIStyle();
        nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node0.png") as Texture2D;
        nodeStyle.border = new RectOffset(12, 12, 12, 12);
        nodeStyle.alignment = TextAnchor.MiddleCenter;

        connectorStyle = new GUIStyle();
        connectorStyle.normal.background = EditorGUIUtility.Load("Node_Arrow.png") as Texture2D;
    }

    private void OnGUI()
    {
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        DrawNodes();

        ProcessNodeEvents(Event.current);
        ProcessEvents(Event.current);


        ScaleWindow();

        if (GUI.changed)
        {
            Repaint();
        }
    }

    private void DrawNodes()
    {
        if(nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Draw();
            }
        }
    }

    //private void DrawConnections()
    //{
    //    if(nodes != null)
    //    {
    //        for(int i = 0; i < nodes.Count; i++)
    //        {

    //        }
    //    }
    //}

    private bool CheckForMenus()
    {
        if(nodes != null)
        {
            foreach (WorldNode node in nodes)
            {
                if (node.createdMenu)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void ProcessEvents(Event e)
    {
        bool otherMenu = CheckForMenus();

        switch (e.type)
        {
            case (EventType.MouseDown):
                if(e.button == 0)
                {
                    scale *= 2;
                }
                if(e.button == 1 && !otherMenu)
                {
                    ProcessContextMenu(e.mousePosition);
                }
                break;

            case (EventType.MouseDrag):
                if(e.button == 2)
                {
                    OnDrag(e.delta);
                }
                break;
        }
    }

    private void ProcessNodeEvents(Event e)
    {
        if(nodes != null)
        {
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                bool guiChanged = nodes[i].ProcessEvents(e);

                if (guiChanged)
                {
                    GUI.changed = true;
                }
            }
        }
    }

    private void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePosition));
        genericMenu.ShowAsContext();
    }

    private void OnClickAddNode(Vector2 mousePosition)
    {
        if(nodes == null)
        {
            nodes = new List<WorldNode>();
        }

        nodes.Add(new WorldNode(mousePosition, 200, 50, nodeStyle, nodeStyle, connectorStyle));
    }

    private void ScaleWindow()
    {
        GUIUtility.ScaleAroundPivot(new Vector2(scale, scale), new Vector2(position.x + position.width / 2, position.y + position.height / 2));
    }

    private void OnDrag(Vector2 delta)
    {
        drag += delta;

        if (nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Drag(delta);
            }
        }

        GUI.changed = true;
    }

    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        Vector2 offset = drag;
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }

        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }
}
