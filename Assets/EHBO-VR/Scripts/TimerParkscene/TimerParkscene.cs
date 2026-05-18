using UnityEngine;
using TMPro;

public class TimerParkscene : MonoBehaviour
{
    [SerializeField] GameObject timerCanvas;
    [SerializeField] TextMeshProUGUI timerparksceneText;
    [SerializeField] float remainingTime = 240f;
    [SerializeField] RectTransform redBar;
    
    [Header("Audio")]
    [SerializeField] private AudioSource timerAudioSource; // Sleep hier je AudioSource in
    [SerializeField] private AudioClip timerTickClip;     // Optioneel: als je een specifieke clip wilt toewijzen

    [Header("References")]
    [SerializeField] IncidentCountdown incidentScript; 
    [SerializeField] EHBOStappenChecker stappenChecker; 

    private bool isTimerRunning = false;
    private float totalTime;

    void Start()
    {
        if (timerCanvas != null)
            timerCanvas.SetActive(false);

        totalTime = remainingTime;
        UpdateTimerDisplay();

        // Zorg dat het geluid niet al speelt bij het opstarten
        if (timerAudioSource != null)
        {
            timerAudioSource.Stop(); 
            timerAudioSource.loop = true; // Meestal wil je dat een timer-geluid herhaalt
        }
    }

    public void StartRealTimer()
    {
        isTimerRunning = true;
        if (timerCanvas != null) 
            timerCanvas.SetActive(true);
        
        // --- NIEUW: Start het geluid ---
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

        Debug.Log("De 300 seconden timer is gestart!");
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

                // --- NIEUW: Stop het geluid als de tijd op is ---
                if (timerAudioSource != null)
                {
                    timerAudioSource.Stop();
                }

                UpdateTimerDisplay();
                UpdateRedBar();
            }
        }
    }

    // De rest van je functies (UpdateRedBar, UpdateTimerDisplay, OnTriggerEnter) blijven hetzelfde...
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