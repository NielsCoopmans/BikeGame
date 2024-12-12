using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

public class NewBehaviourScript : EditorWindow
{
    public Transform waypointRoot;
    private SerializedObject serializedObject;

    [MenuItem("Tools/Waypoint Editor")]
    public static void Open()
    {
        GetWindow<NewBehaviourScript>().titleContent = new GUIContent("Waypoint Editor");
    }

    private void OnEnable()
    {
        // Initialize SerializedObject to access properties
        serializedObject = new SerializedObject(this);
    }

    private void OnGUI()
    {
        serializedObject.Update();

        // Draw the property field for waypointRoot
        SerializedProperty waypointRootProp = serializedObject.FindProperty("waypointRoot");
        EditorGUILayout.PropertyField(waypointRootProp);

        if (waypointRoot == null)
        {
            EditorGUILayout.HelpBox("Root transform must be selected. Please assign a root transform.", MessageType.Warning);
        }
        else
        {
            EditorGUILayout.BeginVertical("box");
            DrawButtons();
            EditorGUILayout.EndVertical();
        }

        serializedObject.ApplyModifiedProperties();
    }

    void DrawButtons()
    {
        if (GUILayout.Button("Create Waypoint"))
        {
            CreateWaypoint();
        }

        // Check if a waypoint is selected
        if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Waypoint>())
        {
            if(GUILayout.Button("Add Branch Waypoint"))
            {
                CreateBranch();
            }
            if (GUILayout.Button("Create Waypoint Before"))
            {
                CreateWaypointBefore();
            }
            if (GUILayout.Button("Create Waypoint After"))
            {
                CreateWaypointAfter();
            }
            if (GUILayout.Button("Remove Waypoint"))
            {
                RemoveWaypoint();
            }
        }
    }

    void CreateWaypoint()
    {
        if (waypointRoot == null)
        {
            UnityEngine.Debug.LogWarning("Waypoint Root is not assigned.");
            return;
        }

        // Create a new GameObject with a Waypoint component
        GameObject waypointObject = new GameObject("Waypoint " + waypointRoot.childCount, typeof(Waypoint));
        waypointObject.transform.SetParent(waypointRoot, false);

        Waypoint waypoint = waypointObject.GetComponent<Waypoint>();
        if (waypointRoot.childCount > 1)
        {
            Waypoint previousWaypoint = waypointRoot.GetChild(waypointRoot.childCount - 2).GetComponent<Waypoint>();
            waypoint.previousWaypoint = previousWaypoint;
            previousWaypoint.nextWaypoint = waypoint;

            // Position the new waypoint at the previous waypoint's position
            waypoint.transform.position = previousWaypoint.transform.position;
            waypoint.transform.forward = previousWaypoint.transform.forward;
        }

        // Set the new waypoint as the active object in the editor
        Selection.activeGameObject = waypointObject;
    }

    void CreateWaypointBefore()
    {
        GameObject waypointObject = new GameObject("Waypoint " + waypointRoot.childCount, typeof(Waypoint));
        waypointObject.transform.SetParent(waypointRoot, false);

        Waypoint newWaypoint = waypointObject.GetComponent<Waypoint>();
        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

        waypointObject.transform.position = selectedWaypoint.transform.position;
        waypointObject.transform.forward = selectedWaypoint.transform.forward;

        if (selectedWaypoint.previousWaypoint != null)
        {
            newWaypoint.previousWaypoint = selectedWaypoint.previousWaypoint;
            selectedWaypoint.previousWaypoint.nextWaypoint = newWaypoint;
        }

        newWaypoint.nextWaypoint = selectedWaypoint;
        selectedWaypoint.previousWaypoint = newWaypoint;

        newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());

        Selection.activeGameObject = newWaypoint.gameObject;
    }

    void CreateWaypointAfter()
    {
        GameObject waypointObject = new GameObject("Waypoint " + waypointRoot.childCount, typeof(Waypoint));
        waypointObject.transform.SetParent(waypointRoot, false);

        Waypoint newWaypoint = waypointObject.GetComponent<Waypoint>();
        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

        waypointObject.transform.position = selectedWaypoint.transform.position;
        waypointObject.transform.forward = selectedWaypoint.transform.forward;

        if (selectedWaypoint.nextWaypoint != null)
        {
            selectedWaypoint.nextWaypoint.previousWaypoint = newWaypoint;
            newWaypoint.nextWaypoint = selectedWaypoint.nextWaypoint;
        }

        selectedWaypoint.nextWaypoint = newWaypoint;

        newWaypoint.previousWaypoint = selectedWaypoint;

        newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex() + 1);

        Selection.activeGameObject = newWaypoint.gameObject;
    }

    void RemoveWaypoint()
    {
        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

        if (selectedWaypoint.nextWaypoint != null)
        {
            selectedWaypoint.nextWaypoint.previousWaypoint = selectedWaypoint.previousWaypoint;
        }
        if (selectedWaypoint.previousWaypoint != null)
        {
            selectedWaypoint.previousWaypoint.nextWaypoint = selectedWaypoint.nextWaypoint;
            Selection.activeGameObject = selectedWaypoint.previousWaypoint.gameObject;
        }

        DestroyImmediate(selectedWaypoint.gameObject);
    }

    void CreateBranch()
    {
        // Check if waypointRoot is assigned
        if (waypointRoot == null)
        {
            UnityEngine.Debug.LogError("waypointRoot is null! Assign it before calling CreateBranch.");
            return;
        }

        // Check if an active GameObject is selected
        if (Selection.activeGameObject == null)
        {
            UnityEngine.Debug.LogError("No active GameObject selected in the Scene!");
            return;
        }

        // Check if the selected GameObject has a Waypoint component
        Waypoint branchedFrom = Selection.activeGameObject.GetComponent<Waypoint>();
        if (branchedFrom == null)
        {
            UnityEngine.Debug.LogError("The selected GameObject does not have a Waypoint component!");
            return;
        }

        //create a new Waypoint GameObject
        GameObject waypointObject = new GameObject("Waypoint " + waypointRoot.childCount, typeof(Waypoint));
        waypointObject.transform.SetParent(waypointRoot, false);

        //get the Waypoint component of the new GameObject
        Waypoint waypoint = waypointObject.GetComponent<Waypoint>();

        //ensure the 'branches' list is initialized
        if (branchedFrom.branches == null)
        {
            branchedFrom.branches = new List<Waypoint>();
        }

        //add the new Waypoint to the branches of the selected Waypoint
        branchedFrom.branches.Add(waypoint);

        //set the position and forward direction of the new Waypoint
        waypoint.transform.position = branchedFrom.transform.position;
        waypoint.transform.forward = branchedFrom.transform.forward;

        //set the new Waypoint as the active GameObject
        Selection.activeGameObject = waypoint.gameObject;
    }

}