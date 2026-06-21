using UnityEngine;
using UnityEngine.SceneManagement;

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

    [Header("UI Eindscherm Settings")]
    [Tooltip("Sleep hier het GameObject van je Eindscherm Canvas in")]
    [SerializeField] private GameObject eindschermCanvas;

    private bool magRennen = false;
    private bool alAangekomen = false; // EXTRA CHECK: Voorkomt dat hij gaat shaken/twijfelen

    void Start()
    {
        if (eindschermCanvas != null)
        {
            eindschermCanvas.SetActive(false);
        }
    }

    public void StartOverdracht()
    {
        magRennen = true;
        alAangekomen = false;

        if (npcAnimator != null)
        {
            npcAnimator.SetTrigger(jogTriggerName);
            Debug.Log("<color=green>[BROEDER]</color> Jog animatie gestart!");
        }
    }

    void Update()
    {
        if (!magRennen || doelWaypoint == null || alAangekomen) return;

        float afstand = Vector3.Distance(transform.position, doelWaypoint.position);

        // We vergroten de marge iets (0.25f) zodat hij gegarandeerd stopt zodra hij er vlakbij is
        if (afstand > 0.25f)
        {
            transform.position = Vector3.MoveTowards(transform.position, doelWaypoint.position, renSnelheid * Time.deltaTime);

            Vector3 richting = doelWaypoint.position - transform.position;
            richting.y = 0; 
            if (richting != Vector3.zero)
            {
                Quaternion doelRotatie = Quaternion.LookRotation(richting);
                transform.rotation = Quaternion.Slerp(transform.rotation, doelRotatie, rotatieSnelheid * Time.deltaTime);
            }
        }
        else
        {
            ArriveerBijSlachtoffer();
        }
    }

    private void ArriveerBijSlachtoffer()
    {
        alAangekomen = true; // Zet de beweging direct KEIHARD op slot
        magRennen = false; 

        // Direct naar de exacte positie en rotatie zetten om op-en-neer lopen te voorkomen
        if (doelWaypoint != null)
        {
            transform.position = doelWaypoint.position;
            transform.rotation = doelWaypoint.rotation;
        }

        if (npcAnimator != null)
        {
            npcAnimator.ResetTrigger(jogTriggerName); // Zorg dat de jog-trigger écht uit staat
            npcAnimator.SetTrigger(knielTriggerName);
            Debug.Log("<color=green>[BROEDER]</color> Bestemming bereikt! Rotatie overgenomen en direct geknield.");
        }

        StartCoroutine(WachtEnToonEindscherm());
    }

    private System.Collections.IEnumerator WachtEnToonEindscherm()
    {
        yield return new WaitForSeconds(5.0f);

        if (eindschermCanvas != null)
        {
            eindschermCanvas.SetActive(true);
        }
    }

    public void KnopHerstartScenario()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void KnopStopApplicatie()
    {
        Application.Quit();
    }
}