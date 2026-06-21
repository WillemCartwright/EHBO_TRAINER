using UnityEngine;
using TMPro;
using System.Collections;

public class TimerParkscene : MonoBehaviour
{
    [SerializeField] GameObject timerCanvas;
    [SerializeField] TextMeshProUGUI timerparksceneText;
    [SerializeField] float remainingTime = 240f;
    [SerializeField] RectTransform redBar;
    
    [Header("Audio")]
    [SerializeField] private AudioSource timerAudioSource; 
    [SerializeField] private AudioClip timerTickClip;     

    [Header("References")]
    [SerializeField] IncidentCountdown incidentScript; 
    [SerializeField] EHBOStappenChecker stappenChecker; 
    [SerializeField] GameObject clipboardGrabableObject; 

    // --- NIEUW: Referentie naar het Fail Canvas ---
    [Header("Fail Scenario Settings")]
    [Tooltip("Sleep hier het speciale Fail Canvas van je reanimatie in")]
    [SerializeField] private GameObject failCanvas;

    private bool isTimerRunning = false;
    private float totalTime;

    void Start()
    {
        if (timerCanvas != null)
        {
            timerCanvas.SetActive(false); 
            Debug.Log("[TIMER] Tijdsbalk tijdelijk verborgen tot het incident start.");
        }

        // Zorg ervoor dat het Fail Canvas bij het opstarten altijd netjes uit staat
        if (failCanvas != null)
        {
            failCanvas.SetActive(false);
        }
        
        totalTime = remainingTime;
        UpdateTimerDisplay();

        if (timerAudioSource != null)
        {
            timerAudioSource.Stop(); 
            timerAudioSource.loop = true; 
        }
    }

    public void StartRealTimer()
    {
        isTimerRunning = true;
        
        StartCoroutine(WachtEnActiveerTijdsbalk(5.0f));
        
        if (timerAudioSource != null)
        {
            if (timerTickClip != null) timerAudioSource.clip = timerTickClip;
            timerAudioSource.Play();
            Debug.Log("Timer geluid en logica gestart!");
        }

        if (stappenChecker != null)
        {
            stappenChecker.VictimHasFallen(); 
        }

        Debug.Log("De 240 seconden timer loopt op de achtergrond. Visuele balk verschijnt over 5 seconden.");
    }

    private IEnumerator WachtEnActiveerTijdsbalk(float delay)
    {
        yield return new WaitForSeconds(delay); 

        if (timerCanvas != null)
        {
            timerCanvas.SetActive(true);
            Debug.Log("[TIMER] De 5 seconden zijn voorbij! Tijdsbalk is nu zichtbaar op de center anchor.");
        }
    }

    private IEnumerator WachtEnActiveerKlembord(float delay)
    {
        yield return new WaitForSeconds(delay); 

        if (clipboardGrabableObject != null)
        {
            clipboardGrabableObject.SetActive(true); 
            Debug.Log("[TIMER] Klembord is nu actief op de grond!");
        }

        if (timerCanvas != null)
        {
            timerCanvas.SetActive(true);
            Debug.Log("[TIMER] Timer op het klembord geforceerd geactiveerd!");
        }
    }

    void Update()
    {
        if (isTimerRunning)
        {
            if (remainingTime > 0)
            {
                remainingTime -= Time.deltaTime;
                UpdateTimerDisplay();
                UpdateRedBar();
            }
            else
            {
                remainingTime = 0;
                isTimerRunning = false;
                timerparksceneText.color = Color.red;

                // --- HIER GAAT HET MIS: Zorg dat het geluid hier ALTIJD stopt! ---
                if (timerAudioSource != null)
                {
                    timerAudioSource.Stop();
                    Debug.Log("[TIMER] Tijd is op, tikgeluid geforceerd uitgezet!");
                }

                UpdateTimerDisplay();
                UpdateRedBar();

                // Controleer of de speler op tijd was, anders Fail Canvas
                EvalueerScenarioTijdOm();
            }
        }
    }

    // --- NIEUW: Check of de speler al bij de ambulance was ---
    private void EvalueerScenarioTijdOm()
    {
        if (stappenChecker != null)
        {
            // We halen de huidige actieve stap op uit jouw EHBOStappenChecker
            string huidigeStap = stappenChecker.GetCurrentStep();

            // Als de huidige stap NIET de ambulance-overdracht is, heeft de speler het helaas niet gehaald
            if (huidigeStap != "Hulpverleners nemen over")
            {
                Debug.Log("<color=red>[FAIL]</color> Tijd is op! Speler was nog bij stap: " + huidigeStap);
                
                // Activeer het Fail Canvas!
                if (failCanvas != null)
                {
                    failCanvas.SetActive(true);
                }
            }
            else
            {
                Debug.Log("<color=green>[SUCCESS]</color> Tijd is om, maar de ambulance was al geactiveerd. Geen fail!");
            }
        }
    }

    // --- NIEUW: Functie om de timer geforceerd stil te zetten bij succes ---
    public void StopTimerBijSucces()
    {
        isTimerRunning = false; // Stop de Update-loop
        
        if (timerAudioSource != null)
        {
            timerAudioSource.Stop(); // Stop direct het tikgeluid
            Debug.Log("[TIMER] Geluid stilgezet omdat het scenario is gehaald!");
        }

        if (timerCanvas != null)
        {
            timerCanvas.SetActive(false); // Verberg de rode tijdsbalk voor het gezicht
            Debug.Log("[TIMER] Visuele balk verborgen bij succes.");
        }
    }

    void UpdateRedBar()
    {
        float fraction = remainingTime / totalTime;
        redBar.localScale = new Vector3(fraction, 1f, 1f);
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerparksceneText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (incidentScript != null)
            {
                incidentScript.Activate();
            }
            GetComponent<Collider>().enabled = false;
        }
    }
}