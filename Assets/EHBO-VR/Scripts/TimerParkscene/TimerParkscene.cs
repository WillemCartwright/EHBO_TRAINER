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
        // We zetten de timer in het script niet meer handmatig uit! 
        // Zorg dat het klembord en de timer in de Unity Editor (Inspector) gewoon AAN staan.
        // Dit script zet de PARENT (het klembord) uit, waardoor de timer automatisch meegaat.
        if (clipboardGrabableObject != null)
            clipboardGrabableObject.SetActive(false);

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
        
        // --- GEWIJZIGD: We halen de timerCanvas.SetActive(true) hier WEG! ---
        // Die mag pas over 5 seconden aan als het klembord er ook is.
        
        if (timerAudioSource != null)
        {
            if (timerTickClip != null) timerAudioSource.clip = timerTickClip;
            timerAudioSource.Play();
            Debug.Log("Timer geluid gestart!");
        }

        if (stappenChecker != null)
        {
            stappenChecker.VictimHasFallen(); 
        }

        // Start de timer die 5 seconden wacht voor het klembord én de timer
        StartCoroutine(WachtEnActiveerKlembord(5.0f));

        Debug.Log("De 300 seconden timer is gestart!");
    }

    private IEnumerator WachtEnActiveerKlembord(float delay)
    {
        yield return new WaitForSeconds(delay); 

        // Zet nu in één klap de ouder aan. Omdat de timer erin zit en in de editor aan staat,
        // komt hij nu direct perfect en synchroon mee tevoorschijn!
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