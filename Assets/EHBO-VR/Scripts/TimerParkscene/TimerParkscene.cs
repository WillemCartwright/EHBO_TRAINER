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

    private bool isTimerRunning = false;
    private float totalTime;

    void Start()
    {
        // --- DE CLIPBOARD & TIMER SPLITSING ---
        // 1. Het klembord blijft lekker AAN vanaf het begin
        // (Dus GEEN clipboardGrabableObject.SetActive(false); meer!)

        // 2. De tijdsbalk voor je gezicht zetten we bij de start juist wél UIT!
        if (timerCanvas != null)
        {
            timerCanvas.SetActive(false); 
            Debug.Log("[TIMER] Tijdsbalk tijdelijk verborgen tot het incident start.");
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
        
        // --- GEWIJZIGD: We zetten de tijdsbalk hier NIET meer direct aan! ---
        // In plaats daarvan starten we de timer die 5 seconden wacht.
        StartCoroutine(WachtEnActiveerTijdsbalk(5.0f));
        
        if (timerAudioSource != null)
        {
            if (timerTickClip != null) timerAudioSource.clip = timerTickClip;
            timerAudioSource.Play();
            Debug.Log("Timer geluid en logica gestart!");
        }

        if (stappenChecker != null)
        {
            stappenChecker.VictimHasFallen(); // Of VictimHasFallen() hoe hij bij jou heette
        }

        Debug.Log("De 240 seconden timer loopt op de achtergrond. Visuele balk verschijnt over 5 seconden.");
    }

    // --- DE NIEUWE VERTRAGINGS-ROUTINE ---
    private IEnumerator WachtEnActiveerTijdsbalk(float delay)
    {
        // Wacht exact 5 seconden
        yield return new WaitForSeconds(delay); 

        // Zet nu pas de tijdsbalk voor je gezicht aan!
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

                if (timerAudioSource != null)
                {
                    timerAudioSource.Stop();
                }

                UpdateTimerDisplay();
                UpdateRedBar();
            }
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