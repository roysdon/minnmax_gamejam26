using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PathFollower : MonoBehaviour
{
    // Transforms placed in editor form the object's path, visited in order
    public List<Transform> waypoints = new List<Transform>();

    public float speed = 2f;
    // Trigger events once the final waypoint is reached
    public UnityEvent onArrived = new UnityEvent();
    private int nextWaypoint = -1;

    public void StartMoving()
    {
        if (nextWaypoint != -1)
        {
            return;
        }

        nextWaypoint = 0;
    }

    void Update()
    {
        if (nextWaypoint < 0 || nextWaypoint >= waypoints.Count)
        {
            return;
        }

        Transform waypoint = waypoints[nextWaypoint];        
        // Skips a missing waypoint without breaking the pathing or throwing an error
        if (waypoint == null)
        {
            AdvanceWaypoint();
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, waypoint.position, speed * Time.deltaTime);

        if (transform.position == waypoint.position)
        {
            AdvanceWaypoint();
        }
    }

    void AdvanceWaypoint()
    {
        nextWaypoint++;

        if (nextWaypoint >= waypoints.Count)
        {
            onArrived.Invoke();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        // Draws spheres at each waypoint with lines connecting them to show the full path
        Vector3 from = transform.position;
        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i] == null)
            {
                continue;
            }

            Vector3 to = waypoints[i].position;
            Gizmos.DrawLine(from, to);
            Gizmos.DrawWireSphere(to, 0.15f);
            from = to;
        }
    }
}
