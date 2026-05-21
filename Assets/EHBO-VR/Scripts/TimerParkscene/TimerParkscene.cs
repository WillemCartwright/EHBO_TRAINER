using UnityEngine;
using TMPro;
using System.Collections; // <--- C# heeft dit nodig voor de Coroutine (wachttijd)

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
    [SerializeField] GameObject clipboardGrabableObject; // <--- NEW: Sleep hier je 'clipboard grabable' in!

    private bool isTimerRunning = false;
    private float totalTime;

    void Start()
    {
        if (timerCanvas != null)
            timerCanvas.SetActive(false);

        // Zorg dat het klembord onzichtbaar is zodra de scene laadt
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
        if (timerCanvas != null) 
            timerCanvas.SetActive(true);
        
        if (timerAudioSource != null)
        {
            if (timerTickClip != null) timerAudioSource.clip = timerTickClip;
            timerAudioSource.Play();
            Debug.Log("Timer geluid gestart!");
        }

        if (stappenChecker != null)
        {
            stappenChecker.VictimHasFallen(); // De man begint nu te vallen!
        }

        // --- NEW: Start direct de timer die 5 seconden wacht voor het klembord ---
        StartCoroutine(WachtEnActiveerKlembord(5.0f));

        Debug.Log("De 300 seconden timer is gestart!");
    }

    // --- NEW: De wachtfunctie voor het klembord ---
    private IEnumerator WachtEnActiveerKlembord(float delay)
    {
        yield return new WaitForSeconds(delay); // Wacht exact 5 seconden

        if (clipboardGrabableObject != null)
        {
            clipboardGrabableObject.SetActive(true); // Zet hem aan als de man stil ligt!
            Debug.Log("[TIMER] 5 seconden voorbij na de val. Klembord is nu zichtbaar op de grond!");
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