using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

[CustomEditor(typeof(QuestScriptableObject))]
public class CustomQuestEditor : Editor
{
    private const string dialoguePath = "Assets/Scriptable Objects/Dialogue Objects";

    private bool isMainStateFoldout = false;
    private bool isInactiveStateFoldout = false;
    private bool isStartStateFoldout = false;
    private bool isInProgressStateFoldout = false;
    private bool isEndStateFoldout = false;
    private bool isCompletedStateFoldout = false;
    private bool isFailedStateFoldout = false;

    // All vars in the quest scripObj
    // ------------ //
    SerializedProperty idQuestName;                     //string
    SerializedProperty inactiveIdNpcName;               //string
    SerializedProperty startIdNpcName;                  //string
    SerializedProperty inProgressIdNpcName;             //string
    SerializedProperty endIdNpcName;                    //string
    SerializedProperty completedIdNpcName;              //string
    SerializedProperty failedIdNpcName;                 //string
    SerializedProperty idQuestItemNames;                //List<string>

    SerializedProperty displayQuestName;                //string
    SerializedProperty displayNpcName;                  //string
    SerializedProperty displayQuestDescription;         //string
    //SerializedProperty displayQuestItemDescriptions;    //List<string>
    SerializedProperty reputationPoints;                //float

    SerializedProperty initialQuestState;               //QuestState

    //SerializedProperty runOnComplete;                   //UnityEvent
    SerializedProperty unlockOnComplete;                //List<string>

    SerializedProperty inactiveDialogue;                //DialogueScriptableObject
    SerializedProperty startDialogue;                   //DialogueScriptableObject
    SerializedProperty inProgressDialogue;              //DialogueScriptableObject
    SerializedProperty endDialogue;                     //DialogueScriptableObject
    SerializedProperty completedDialogue;               //DialogueScriptableObject
    SerializedProperty failedDialogue;                  //DialogueScriptableObject

    SerializedProperty onInactiveToStart;               //List<FunctionParams>
    SerializedProperty onStartToInProgress;             //List<FunctionParams>
    SerializedProperty onInProgressToEnd;               //List<FunctionParams>
    SerializedProperty onEndToCompleted;                //List<FunctionParams>
    SerializedProperty onStateToFailed;                 //List<FunctionParams>
    // ------------ //

    private void OnEnable()
    {
        idQuestName = serializedObject.FindProperty("idQuestName");
        inactiveIdNpcName = serializedObject.FindProperty("inactiveIdNpcName");
        startIdNpcName = serializedObject.FindProperty("startIdNpcName");
        inProgressIdNpcName = serializedObject.FindProperty("inProgressIdNpcName");
        endIdNpcName = serializedObject.FindProperty("endIdNpcName");
        completedIdNpcName = serializedObject.FindProperty("completedIdNpcName");
        failedIdNpcName = serializedObject.FindProperty("endIdNpcName");
        idQuestItemNames = serializedObject.FindProperty("idQuestItemNames");

        displayQuestName = serializedObject.FindProperty("displayQuestName");
        displayNpcName = serializedObject.FindProperty("displayNpcName");
        displayQuestDescription = serializedObject.FindProperty("displayQuestDescription");
        //displayQuestItemDescriptions = serializedObject.FindProperty("displayQuestItemDescriptions");
        reputationPoints = serializedObject.FindProperty("reputationPoints");

        initialQuestState = serializedObject.FindProperty("initialQuestState");

        //runOnComplete = serializedObject.FindProperty("runOnComplete");
        unlockOnComplete = serializedObject.FindProperty("unlockOnComplete");

        inactiveDialogue = serializedObject.FindProperty("inactiveDialogue");
        startDialogue = serializedObject.FindProperty("startDialogue");
        inProgressDialogue = serializedObject.FindProperty("inProgressDialogue");
        endDialogue = serializedObject.FindProperty("endDialogue");
        completedDialogue = serializedObject.FindProperty("completedDialogue");
        failedDialogue = serializedObject.FindProperty("failedDialogue");

        onInactiveToStart = serializedObject.FindProperty("onInactiveToStart");
        onStartToInProgress = serializedObject.FindProperty("onStartToInProgress");
        onInProgressToEnd = serializedObject.FindProperty("onInProgressToEnd");
        onEndToCompleted = serializedObject.FindProperty("onEndToCompleted");
        onStateToFailed = serializedObject.FindProperty("onStateToFailed");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUILayout.Label("Settings", EditorStyles.boldLabel);
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();
        #region IDs
        #region Quest ID
        GUILayout.Label("ID of Quest name", EditorStyles.label);
        idQuestName.stringValue = EditorGUILayout.TextField(idQuestName.stringValue);
        #endregion

        GUILayout.Space(10);

        #region NPC IDs
        GUILayout.Label("IDs of NPC names", EditorStyles.label);

        GUILayout.Label("Inactive NPC ID", EditorStyles.label);
        inactiveIdNpcName.stringValue = EditorGUILayout.TextField(inactiveIdNpcName.stringValue);
        GUILayout.Space(10);

        GUILayout.Label("Start NPC ID", EditorStyles.label);
        startIdNpcName.stringValue = EditorGUILayout.TextField(startIdNpcName.stringValue);
        GUILayout.Space(10);

        GUILayout.Label("In Progress NPC ID", EditorStyles.label);
        inProgressIdNpcName.stringValue = EditorGUILayout.TextField(inProgressIdNpcName.stringValue);
        GUILayout.Space(10);

        GUILayout.Label("End NPC ID", EditorStyles.label);
        endIdNpcName.stringValue = EditorGUILayout.TextField(endIdNpcName.stringValue);
        GUILayout.Space(10);

        GUILayout.Label("Completed NPC ID", EditorStyles.label);
        completedIdNpcName.stringValue = EditorGUILayout.TextField(completedIdNpcName.stringValue);
        GUILayout.Space(10);

        GUILayout.Label("Failed NPC ID", EditorStyles.label);
        failedIdNpcName.stringValue = EditorGUILayout.TextField(failedIdNpcName.stringValue);
        #endregion

        GUILayout.Space(10);

        #region Quest Item IDs
        GUILayout.Label("IDs of necessary items for quest", EditorStyles.label);
        EditorGUILayout.PropertyField(idQuestItemNames);
        #endregion
        #endregion
        GUILayout.EndVertical();

        //GUILayout.Space(10);

        GUILayout.BeginVertical();
        #region Notebook Display Values
        #region Quest Name
        GUILayout.Label("Display name of Quest");
        displayQuestName.stringValue = EditorGUILayout.TextField(displayQuestName.stringValue);
        #endregion

        GUILayout.Space(10);

        #region NPC Name
        GUILayout.Label("Display name of NPC");
        displayNpcName.stringValue = EditorGUILayout.TextField(displayNpcName.stringValue);
        #endregion

        GUILayout.Space(10);

        //UNUSED (Use the IDs to display in questbook)
        #region Item Descriptions
        //GUILayout.Label("Displayed description of Quest Items");
        //EditorGUILayout.PropertyField(displayQuestItemDescriptions);
        #endregion

        GUILayout.Space(10);

        #region Quest Description
        GUILayout.Label("Displayed description of Quest");
        displayQuestDescription.stringValue = EditorGUILayout.TextArea(displayQuestDescription.stringValue);
        #endregion
        #endregion
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        
        #region Quest State
        EditorGUILayout.PropertyField(initialQuestState);
        #endregion

        GUILayout.Space(10);

        //UNUSED (Check below)
        #region Events - Run On Complete
        //EditorGUILayout.PropertyField(runOnComplete);
        #endregion

        GUILayout.Space(10);

        #region Unlock On Complete
        GUILayout.Label("Names of quests to unlock on complete (could be moved to RunOnComplete)", EditorStyles.label);
        EditorGUILayout.PropertyField(unlockOnComplete);
        #endregion

        GUILayout.Space(10);

        #region Reputation Points
        GUILayout.Label("Points given upon completion of quest", EditorStyles.label);
        reputationPoints.floatValue = EditorGUILayout.FloatField(reputationPoints.floatValue);
        #endregion

        GUILayout.Space(10);

        isMainStateFoldout = EditorGUILayout.Foldout(isMainStateFoldout, "Quest States", true);

        if (isMainStateFoldout)
        {
            #region Dialogues (actually gonna have more)
            //GUILayout.Label("Dialogues", EditorStyles.boldLabel);
            


            isInactiveStateFoldout = EditorGUILayout.Foldout(isInactiveStateFoldout, "Inactive", true);

            if (isInactiveStateFoldout)
            {
                #region Inactive
                #region Dialogue
                GUILayout.Label("Inactive Dialogue", EditorStyles.boldLabel);
                GUILayout.Label("Can't get rid of the label that says \"Dialogue\" for some reason, it was auto-added by unity and I can't fix it", EditorStyles.miniLabel);

                GUILayout.BeginHorizontal();

                EditorGUILayout.PropertyField(inactiveDialogue);

                if (GUILayout.Button("Inspect"))
                {
                    OpenNewDialogueInspector(inactiveDialogue, "Inactive");
                }
                GUILayout.EndHorizontal();
                #endregion

                #region Events
                EditorGUILayout.PropertyField(onInactiveToStart);
                #endregion

                #endregion
            }

            GUILayout.Space(10);


            isStartStateFoldout = EditorGUILayout.Foldout(isStartStateFoldout, "Start", true);

            if (isStartStateFoldout)
            {
                #region Start
                #region Dialogue
                GUILayout.Label("Start Dialogue", EditorStyles.boldLabel);
                GUILayout.BeginHorizontal();

                EditorGUILayout.PropertyField(startDialogue);

                if (GUILayout.Button("Inspect"))
                {
                    OpenNewDialogueInspector(startDialogue, "Start");
                }
                GUILayout.EndHorizontal();
                #endregion

                #region Events
                EditorGUILayout.PropertyField(onStartToInProgress);
                #endregion


                #endregion
            }

            GUILayout.Space(10);


            isInProgressStateFoldout = EditorGUILayout.Foldout(isInProgressStateFoldout, "In Progress", true);

            if (isInProgressStateFoldout)
            {
                #region InProgress
                #region Dialogue
                GUILayout.Label("InProgress Dialogue", EditorStyles.boldLabel);
                GUILayout.BeginHorizontal();

                EditorGUILayout.PropertyField(inProgressDialogue);

                if (GUILayout.Button("Inspect"))
                {
                    OpenNewDialogueInspector(inProgressDialogue, "InProgress");
                }
                GUILayout.EndHorizontal();
                #endregion

                #region Events
                EditorGUILayout.PropertyField(onInProgressToEnd);
                #endregion


                #endregion
            }

            GUILayout.Space(10);


            isEndStateFoldout = EditorGUILayout.Foldout(isEndStateFoldout, "End", true);

            if (isEndStateFoldout)
            {
                #region End
                #region Dialogue
                GUILayout.Label("End Dialogue", EditorStyles.boldLabel);
                GUILayout.BeginHorizontal();

                EditorGUILayout.PropertyField(endDialogue);

                if (GUILayout.Button("Inspect"))
                {
                    OpenNewDialogueInspector(endDialogue, "End");
                }
                GUILayout.EndHorizontal();
                #endregion

                #region Events
                EditorGUILayout.PropertyField(onEndToCompleted);
                #endregion


                #endregion
            }


            GUILayout.Space(10);


            isCompletedStateFoldout = EditorGUILayout.Foldout(isCompletedStateFoldout, "Completed", true);

            if (isCompletedStateFoldout)
            {
                #region Completed
                #region Dialogue
                GUILayout.Label("Completed Dialogue", EditorStyles.boldLabel);
                GUILayout.BeginHorizontal();

                EditorGUILayout.PropertyField(completedDialogue);

                if (GUILayout.Button("Inspect"))
                {
                    OpenNewDialogueInspector(completedDialogue, "Completed");
                }
                GUILayout.EndHorizontal();
                #endregion

                #region Events (UNUSED)

                #endregion
                #endregion
            }

            GUILayout.Space(10);


            isFailedStateFoldout = EditorGUILayout.Foldout(isFailedStateFoldout, "Failed", true);

            if (isFailedStateFoldout)
            {
                #region Failed
                #region Dialogue
                GUILayout.Label("Failed Dialogue", EditorStyles.boldLabel);
                GUILayout.BeginHorizontal();

                EditorGUILayout.PropertyField(failedDialogue);

                if (GUILayout.Button("Inspect"))
                {
                    OpenNewDialogueInspector(failedDialogue, "Failed");
                }
                GUILayout.EndHorizontal();
                #endregion

                #region Events
                EditorGUILayout.PropertyField(onStateToFailed);
                #endregion


                #endregion
            }
            #endregion
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void OpenNewDialogueInspector(SerializedProperty prop, string path)
    {
        ActiveEditorTracker.sharedTracker.isLocked = true;

        Object dialogue;

        if (prop.objectReferenceValue == null)
        {
            if (!AssetDatabase.IsValidFolder(dialoguePath + "/" + serializedObject.targetObject.name))
            {
                AssetDatabase.CreateFolder(dialoguePath, serializedObject.targetObject.name);
            }

            DialogueScriptableObject asset = ScriptableObject.CreateInstance<DialogueScriptableObject>();

            AssetDatabase.CreateAsset(asset, dialoguePath + "/" + serializedObject.targetObject.name + "/" + serializedObject.targetObject.name + " " + path + " Dialogue.asset");
            AssetDatabase.SaveAssets();

            prop.objectReferenceValue = asset;

            dialogue = asset;
        }
        else
        {
            dialogue = prop.objectReferenceValue;
        }

        EditorWindow inspectorWindow = Instantiate(EditorWindow.GetWindow(typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow")));
        inspectorWindow.Focus();
        AssetDatabase.OpenAsset(dialogue);
    }
}
