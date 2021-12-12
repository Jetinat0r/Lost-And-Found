using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public class WorldNodeEditor : EditorWindow
{
    private WorldObject curWorld;

    private string path = "Assets/Scriptable Objects/Worlds";

    //private List<WorldNode> nodes;

    private GUIStyle nodeStyle;
    private GUIStyle selectedNodeStyle;
    private GUIStyle connectorStyle;
    private GUIStyle selectedConnectorStyle;

    //public float scale = 1f;

    //private Vector2 offset = Vector2.zero;

    //TODO: onRightClick && noNodesClicked:
    //move all nodes by delta
    //do not move connections

    [MenuItem("Custom Tools/World Editor")]
    private static void OpenWindow()
    {
        WorldNodeEditor window = GetWindow<WorldNodeEditor>();
        window.titleContent = new GUIContent("World Editor");
    }

    [OnOpenAssetAttribute(1)]
    public static bool OnOpenAsset(int instanceID, int line)
    {
        string assetPath = AssetDatabase.GetAssetPath(instanceID);
        WorldObject scriptableObject = AssetDatabase.LoadAssetAtPath<WorldObject>(assetPath);
        if (scriptableObject != null)
        {
            WorldNodeEditor window = (WorldNodeEditor)GetWindow(typeof(WorldNodeEditor));
            window.titleContent = new GUIContent("World Editor");

            window.SetCurWorld(scriptableObject);
            window.Show();

            return true;
        }

        return false;
    }

    private void SetCurWorld(WorldObject world)
    {
        curWorld = world;
    }

    private void OnEnable()
    {
        wantsMouseMove = true;

        nodeStyle = new GUIStyle();
        nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node0.png") as Texture2D;
        nodeStyle.border = new RectOffset(12, 12, 12, 12);
        nodeStyle.alignment = TextAnchor.MiddleCenter;
        nodeStyle.normal.textColor = Color.white;

        selectedNodeStyle = new GUIStyle();
        selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node0 on.png") as Texture2D;
        selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);
        selectedNodeStyle.alignment = TextAnchor.MiddleCenter;
        selectedNodeStyle.normal.textColor = Color.white;

        connectorStyle = new GUIStyle();
        connectorStyle.normal.background = EditorGUIUtility.Load("Node_Arrow.png") as Texture2D;

        selectedConnectorStyle = new GUIStyle();
        selectedConnectorStyle.normal.background = EditorGUIUtility.Load("Node_Arrow.png") as Texture2D;

        //Ensure a "Scriptable Objects" folder exists
        if (!AssetDatabase.IsValidFolder("Assets/Scriptable Objects"))
        {
            AssetDatabase.CreateFolder("Assets", "Scriptable Objects");
        }

        //Ensure a "Worlds" folder exists
        if (!AssetDatabase.IsValidFolder(path))
        {
            AssetDatabase.CreateFolder("Assets/Scriptable Objects", "Worlds");
        }
    }

    private void OnGUI()
    {
        if (curWorld == null)
        {
            return;
        }

        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        DrawConnectors();
        DrawNodes();

        ProcessNodeEvents(Event.current);
        ProcessEvents(Event.current);


        //ScaleWindow();

        if (GUI.changed)
        {
            Repaint();
        }
    }

    private void DrawConnectors()
    {
        if (curWorld.nodes != null)
        {
            for (int i = 0; i < curWorld.nodes.Count; i++)
            {
                curWorld.nodes[i].DrawConnectors();
            }
        }
    }

    private void DrawNodes()
    {
        if(curWorld.nodes != null)
        {
            for (int i = 0; i < curWorld.nodes.Count; i++)
            {
                curWorld.nodes[i].Draw();
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

    private void ProcessEvents(Event e)
    {
        EditorUtility.SetDirty(curWorld);

        if(e.type == EventType.Used)
        {
            return;
        }

        switch (e.type)
        {
            case (EventType.KeyDown):
                if (e.keyCode == KeyCode.Z)
                {
                    foreach(WorldNode node in curWorld.nodes)
                    {
                        ScriptableObject.DestroyImmediate(node);
                    }

                    curWorld.nodes = new List<WorldNode>();
                }
                break;

            case (EventType.MouseDown):
                //if(e.button == 0)
                //{
                //    //curWorld.scale *= 2;
                //}
                if(e.button == 1)
                {
                    ProcessContextMenu(e.mousePosition);
                }
                break;

            case (EventType.MouseDrag):
                if(e.button == 0 || e.button == 2)
                {
                    OnDrag(e.delta);
                }
                break;

            //case (EventType.ScrollWheel):
            //    if(e.delta.y < 0)
            //    {
            //        // Scroll up
            //        // Zoom in
            //        curWorld.scale /= 2;
            //        Zoom(curWorld.scale, e.mousePosition);
            //    }
            //    else
            //    {
            //        // Scroll down
            //        // Zoom out
            //        curWorld.scale *= 2;
            //        Zoom(curWorld.scale, e.mousePosition);
            //    }
            //    break;
        }
    }

    private void ProcessNodeEvents(Event e)
    {
        if(curWorld.nodes != null)
        {
            for (int i = curWorld.nodes.Count - 1; i >= 0; i--)
            {
                bool guiChanged = curWorld.nodes[i].ProcessEvents(e);

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

        if(curWorld.CheckNewConnections() != -1)
        {
            genericMenu.AddItem(new GUIContent("Cancel Connection"), false, () => curWorld.nodes[curWorld.CheckNewConnections()].CancelConnection());
        }

        genericMenu.AddItem(new GUIContent("Add Node"), false, () => OnClickAddNode(mousePosition));
        genericMenu.ShowAsContext();
    }

    private void OnClickAddNode(Vector2 mousePosition)
    {
        if(curWorld.nodes == null)
        {
            curWorld.nodes = new List<WorldNode>();
        }

        WorldNode node = ScriptableObject.CreateInstance<WorldNode>();

        //Check if curWorld's folder exists
        if (!AssetDatabase.IsValidFolder(path + "/"+ curWorld.name))
        {
            AssetDatabase.CreateFolder(path, curWorld.name);
        }

        //Check if curWorld's Node folder exists
        if (!AssetDatabase.IsValidFolder(path + "/" + curWorld.name + "/" + "Nodes"))
        {
            AssetDatabase.CreateFolder(path + "/" + curWorld.name, "Nodes");
        }

        //Can't be null bc one is guaranteed to be there by this step
        string nodeNum = curWorld.nodeCount.ToString();

        //Check if current node's folder exists
        if (!AssetDatabase.IsValidFolder(path + "/" + curWorld.name + "/" + "Nodes" + "/" + "node_" + nodeNum))
        {
            AssetDatabase.CreateFolder(path + "/" + curWorld.name + "/" + "Nodes", "node_" + nodeNum);
        }

        AssetDatabase.CreateAsset(node, path + "/" + curWorld.name + "/" + "Nodes" + "/" + "node_" + nodeNum + "/" + "node_" + nodeNum + ".asset");


        node.SetupNode(curWorld, mousePosition, 200, 50, nodeStyle, selectedNodeStyle, connectorStyle, selectedConnectorStyle, path + "/" + curWorld.name + "/" + "Nodes" + "/" + "node_" + nodeNum);
        curWorld.nodes.Add(node);
        curWorld.nodeCount++;

        EditorUtility.SetDirty(curWorld);
    }

    //private void AddNodeFile(WorldNode worldNode)
    //{
    //    //TODO: take destination and origin titles and add to file output
    //}

    private void ScaleWindow()
    {
        GUIUtility.ScaleAroundPivot(new Vector2(curWorld.scale, curWorld.scale), Vector2.zero);
    }

    private void OnDrag(Vector2 delta)
    {
        curWorld.offset += delta;

        if (curWorld.nodes != null)
        {
            for (int i = 0; i < curWorld.nodes.Count; i++)
            {
                curWorld.nodes[i].Drag(delta * curWorld.scale);
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

        Vector2 gridOffset = curWorld.offset;
        Vector3 newOffset = new Vector3(gridOffset.x % gridSpacing, gridOffset.y % gridSpacing, 0);

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

    private void Zoom(float scale, Vector2 mousePos)
    {
        curWorld.offset = curWorld.offset * scale + mousePos;
    }
}
