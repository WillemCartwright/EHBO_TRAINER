using UnityEngine;

public class HulpverlenerMover : MonoBehaviour
{
    [Header("Animatie Triggers")]
    [SerializeField] private Animator npcAnimator;
    [SerializeField] private string jogTriggerName = "StartJog";
    [SerializeField] private string knielTriggerName = "StartKniel";

    [Header("Beweging Settings")]
    [Tooltip("Het waypoint naast het slachtoffer waar de broeder heen moet rennen")]
    [SerializeField] private Transform doelWaypoint;
    [SerializeField] private float renSnelheid = 3f;
    [SerializeField] private float rotatieSnelheid = 8f;

    private bool magRennen = false;

    // Deze functie wordt straks aangeroepen door de ambulance bij aankomst!
    public void StartOverdracht()
    {
        magRennen = true;

        // Zet de jog-animatie aan in de Animator
        if (npcAnimator != null)
        {
            npcAnimator.SetTrigger(jogTriggerName);
            Debug.Log("<color=green>[BROEDER]</color> Jog animatie gestart!");
        }
    }

    void Update()
    {
        if (!magRennen || doelWaypoint == null) return;

        // Kijk hoe ver we nog van het slachtoffer verwijderd zijn
        float afstand = Vector3.Distance(transform.position, doelWaypoint.position);

        if (afstand > 0.2f)
        {
            // 1. Beweeg richting het slachtoffer waypoint
            transform.position = Vector3.MoveTowards(transform.position, doelWaypoint.position, renSnelheid * Time.deltaTime);

            // 2. Draai netjes richting het slachtoffer waypoint
            Vector3 richting = doelWaypoint.position - transform.position;
            richting.y = 0; // Zorg dat hij niet omhoog of omlaag kantelt
            if (richting != Vector3.zero)
            {
                Quaternion doelRotatie = Quaternion.LookRotation(richting);
                transform.rotation = Quaternion.Slerp(transform.rotation, doelRotatie, rotatieSnelheid * Time.deltaTime);
            }
        }
        else
        {
            // WE ZIJN ER!
            ArriveerBijSlachtoffer();
        }
    }

    private void ArriveerBijSlachtoffer()
    {
        magRennen = false; // Stop de Update-loop

        // Start de knielende animatie in de Animator
        if (npcAnimator != null)
        {
            npcAnimator.SetTrigger(knielTriggerName);
            Debug.Log("<color=green>[BROEDER]</color> Bestemming bereikt! Kniel animatie gestart.");
        }
    }
}