using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    [Header("Route Instellingen")]
    public Transform[] waypoints; 
    public float speed = 5f;       
    public float rotationSpeed = 5f; 

    private int currentWaypointIndex = 0;
    private bool magRijden = false; // Standaard staat de ambulance stil

    // Dit vlaggetje zetten we straks aan vanuit je fasescript
    public void StartRijden()
{
    Debug.Log("<color=magenta>[AMBULANCE]</color> Ik heb het signaal ontvangen en ik ga NU rijden!");
    magRijden = true;
}

    void Update()
    {
        // Als magRijden false is, doet het script niks
        if (!magRijden) return;

        if (waypoints == null || waypoints.Length == 0) return;
        if (currentWaypointIndex >= waypoints.Length) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];

        // Bewegen
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);

        // Draaien
        Vector3 direction = targetWaypoint.position - transform.position;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Volgende waypoint check
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.2f)
        {
            currentWaypointIndex++;
        }
    }
}