using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;

[InitializeOnLoad]
public static class WaypointEditor
{
    // This method draws gizmos in the scene view.
    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
    public static void OnDrawSceneGizmo(Waypoint waypoint, GizmoType gizmoType)
    {
        // Only draw gizmos if we are not in play mode (so it only draws in Scene view, not in Game view)
        if (Application.isPlaying) return;

        // Set color based on whether the waypoint is selected
        if ((gizmoType & GizmoType.Selected) != 0)
        {
            Gizmos.color = Color.yellow;
        }
        else
        {
            Gizmos.color = Color.yellow * 0.5f;
        }

        // Draw the main waypoint sphere
        Gizmos.DrawSphere(waypoint.transform.position, 0.1f);

        // Draw the width line
        Gizmos.color = Color.white;
        Gizmos.DrawLine(
            waypoint.transform.position + (waypoint.transform.right * waypoint.width / 2f),
            waypoint.transform.position - (waypoint.transform.right * waypoint.width / 2f)
        );

        // Draw line to the previous waypoint, if it exists
        if (waypoint.previousWaypoint != null)
        {
            Gizmos.color = Color.red;
            Vector3 offset = waypoint.transform.right * waypoint.width / 2f;
            Vector3 offsetTo = waypoint.previousWaypoint.transform.right * waypoint.previousWaypoint.width / 2f;

            Gizmos.DrawLine(waypoint.transform.position + offset, waypoint.previousWaypoint.transform.position + offsetTo);
        }

        // Draw line to the next waypoint, if it exists
        if (waypoint.nextWaypoint != null)
        {
            Gizmos.color = Color.green;
            Vector3 offset = waypoint.transform.right * -waypoint.width / 2f;
            Vector3 offsetTo = waypoint.nextWaypoint.transform.right * -waypoint.nextWaypoint.width / 2f;

            Gizmos.DrawLine(waypoint.transform.position + offset, waypoint.nextWaypoint.transform.position + offsetTo);
        }

        // Draw branches if they exist
        if (waypoint != null && waypoint.branches != null)
        {
            foreach (Waypoint branch in waypoint.branches)
            {
                // Check if the branch itself is null before accessing it
                if (branch != null)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(waypoint.transform.position, branch.transform.position);
                }
                else
                {
                    UnityEngine.Debug.LogWarning("A branch in waypoint.branches is null.");
                }
            }
        }
        else
        {
            if (waypoint == null)
                UnityEngine.Debug.LogError("Waypoint is null in OnDrawSceneGizmo.");
            else
                UnityEngine.Debug.LogWarning("waypoint.branches is null.");
        }

    }
}
