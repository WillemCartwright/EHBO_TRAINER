using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    [Header("Route Instellingen")]
    public Transform[] waypoints; 
    public float speed = 5f;       
    public float rotationSpeed = 5f; 

    [Header("Audio Settings (Sirene)")]
    [Tooltip("Sleep hier de AudioSource in die op de ambulance staat")]
    [SerializeField] private AudioSource sireneAudioSource;
    [Tooltip("Sleep hier het geluidsbestand van de sirene in")]
    [SerializeField] private AudioClip sireneAudioClip;

    [Header("Hulpverleners Settings")]
    [Tooltip("Sleep hier de ambulancebroeder NPC in die geactiveerd moet worden bij aankomst")]
    [SerializeField] private GameObject hulpverlenerNPC;

    private int currentWaypointIndex = 0;
    private bool magRijden = false; 
    private Transform childMesh;    

    void Start()
    {
        childMesh = transform.Find("ambu2");
        if (childMesh != null)
        {
            childMesh.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("[AMBULANCE] Kon child 'ambu2' niet vinden!");
        }

        if (sireneAudioSource != null)
        {
            sireneAudioSource.loop = true; 
            sireneAudioSource.Stop();
        }

        // VEILIGHEID: Zorg dat de hulpverlener bij het opstarten van de game ALTIJD onzichtbaar is
        if (hulpverlenerNPC != null)
        {
            hulpverlenerNPC.SetActive(false);
        }
    }

    public void StartRijden()
    {
        Debug.Log("<color=magenta>[AMBULANCE]</color> Ik heb het signaal ontvangen en ga NU rijden!");
        
        if (childMesh != null)
        {
            childMesh.gameObject.SetActive(true);
        }

        if (sireneAudioSource != null && sireneAudioClip != null)
        {
            sireneAudioSource.clip = sireneAudioClip;
            sireneAudioSource.Play();
            Debug.Log("<color=magenta>[AMBULANCE]</color> Sirene gestart.");
        }

        magRijden = true;
    }

    void Update()
    {
        if (!magRijden) return;

        if (waypoints == null || waypoints.Length == 0) return;

        if (currentWaypointIndex >= waypoints.Length)
        {
            StopMetRijdenEnGeluid();
            return;
        }

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

    private void StopMetRijdenEnGeluid()
    {
        magRijden = false;

        if (sireneAudioSource != null)
        {
            sireneAudioSource.Stop();
            Debug.Log("<color=magenta>[AMBULANCE]</color> Eindbestemming bereikt, sirene uitgezet.");
        }

        // Activeer de hulpverlener-NPC en geef hem het startsein!
        if (hulpverlenerNPC != null)
        {
            hulpverlenerNPC.SetActive(true);
            Debug.Log("<color=green>[NPC]</color> Hulpverlener is geactiveerd!");

            // --- NIEUW: Zoek het mover-script op de NPC en start de ren-actie ---
            HulpverlenerMover broederScript = hulpverlenerNPC.GetComponent<HulpverlenerMover>();
            if (broederScript != null)
            {
                broederScript.StartOverdracht();
            }
        }
    }
}